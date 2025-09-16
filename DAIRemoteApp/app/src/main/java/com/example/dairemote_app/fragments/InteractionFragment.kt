package com.example.dairemote_app.fragments

import android.annotation.SuppressLint
import android.content.Context
import android.graphics.Color
import android.graphics.Rect
import android.os.Build
import android.os.Bundle
import android.os.Handler
import android.os.VibrationEffect
import android.os.Vibrator
import android.os.VibratorManager
import android.text.Editable
import android.text.TextWatcher
import android.view.GestureDetector
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.MotionEvent
import android.view.View
import android.view.ViewGroup
import android.view.ViewTreeObserver
import android.view.inputmethod.EditorInfo
import android.view.inputmethod.InputMethodManager
import android.widget.Button
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.SeekBar
import android.widget.SeekBar.OnSeekBarChangeListener
import android.widget.TextView
import android.widget.Toast
import androidx.activity.OnBackPressedCallback
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.core.content.ContextCompat
import androidx.core.view.GravityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.dairemote_app.R
import com.example.dairemote_app.databinding.FragmentInteractionBinding
import com.example.dairemote_app.utils.AudioRecyclerAdapter
import com.example.dairemote_app.utils.BackspaceEditText
import com.example.dairemote_app.utils.ConnectionMonitor
import com.example.dairemote_app.utils.DisplayProfilesRecyclerAdapter
import com.example.dairemote_app.utils.KeyboardToolbar
import com.example.dairemote_app.utils.SharedPrefsHelper
import com.example.dairemote_app.viewmodels.ConnectionViewModel
import java.net.SocketException

class InteractionFragment : Fragment() {
    private var _binding: FragmentInteractionBinding? = null
    private val binding get() = _binding!!
    private lateinit var viewModel: ConnectionViewModel
    private var connectionMonitor: ConnectionMonitor? = null
    private lateinit var sharedPrefsHelper: SharedPrefsHelper

    // Other variables from your Activity
    private lateinit var editText: BackspaceEditText
    private lateinit var interactionsHelpText: TextView
    private lateinit var startTutorial: TextView
    private val handler = Handler()
    private lateinit var toolbar: KeyboardToolbar
    private var keyboardLayoutListener: ViewTreeObserver.OnGlobalLayoutListener? = null

    // Audio Control Panel and Host Audio Devices variables
    private lateinit var audioControlPanel: ConstraintLayout
    private lateinit var volumeSlider: SeekBar
    private lateinit var currentVolume: TextView
    private lateinit var expandButton: Button
    private lateinit var expandArrowUp: ImageView
    private lateinit var expandArrowDown: ImageView
    private lateinit var audioRecyclerViewOptions: RecyclerView
    private lateinit var audioRecyclerAdapter: AudioRecyclerAdapter
    private var audioListVisible = false
    private var audioMuted = false
    private lateinit var audioToggleMuteButton: ImageButton

    // Host Display Profiles variables
    private lateinit var displayRecyclerViewOptions: RecyclerView
    private lateinit var displayProfilesRecyclerAdapter: DisplayProfilesRecyclerAdapter
    private var displayListVisible = false

    // Touch interactions variables
    private var startX = 0f
    private var startY = 0f
    private var x = 0f
    private var y = 0f
    private var deltaX = 0f
    private var deltaY = 0f
    private var currentX = 0f
    private var currentY = 0f
    private var deltaT = 0f
    private var startTime: Long = 0
    private val clickThreshold = 125 // Maximum time to register
    private var rmbDetected = false // Suppress movement during rmb
    private var scrolling = false // Suppress other inputs during scroll
    private val mouseSensitivity = 1f
    private var initialPointerCount = 0

    private fun connectionLossHandler(message: String?) {
        cleanUp()

        Toast.makeText(requireContext(), message, Toast.LENGTH_SHORT).show()
        findNavController().navigate(R.id.action_to_main)
    }

    fun messageHost(message: String) {
        if (!viewModel.messageHost(message)) {
            connectionLossHandler("Connection lost")
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        viewModel = ViewModelProvider(requireActivity())[ConnectionViewModel::class.java]

        viewModel.connectionManager.let {
            connectionMonitor = it?.let { it1 -> ConnectionMonitor.getInstance(it1, viewModel) }
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentInteractionBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        interactionsHelpText = binding.interationsHelpTextView
        startTutorial = binding.tutorialStartBtn

        // Audio Control Panel
        volumeSlider = binding.volumeSlider
        volumeSlider.progress = 100
        expandButton = binding.expandAudioButton
        expandArrowUp = binding.expandArrowup
        expandArrowDown = binding.expandArrowdown
        audioToggleMuteButton = binding.audioTogglemuteButton
        currentVolume = binding.audioSliderVolume
        audioControlPanel = binding.audioControlPanel
        audioRecyclerViewOptions = binding.audioRecyclerview

        audioRecyclerViewOptions.setLayoutManager(LinearLayoutManager(requireContext()))
        audioRecyclerAdapter = AudioRecyclerAdapter(ArrayList(), viewModel.connectionManager!!)
        audioRecyclerViewOptions.setAdapter(audioRecyclerAdapter)

        // Display Recycler
        displayRecyclerViewOptions = binding.displayRecyclerview

        displayRecyclerViewOptions.setLayoutManager(LinearLayoutManager(requireContext()))
        displayProfilesRecyclerAdapter =
            DisplayProfilesRecyclerAdapter(
                ArrayList(),
                viewModel.connectionManager!!
            )
        displayRecyclerViewOptions.setAdapter(displayProfilesRecyclerAdapter)
        sharedPrefsHelper = SharedPrefsHelper(requireContext())

        /*        val tutorial = TutorialMediator.GetInstance(AlertDialog.Builder(requireContext()))
                if (tutorial != null) {
                    if (tutorial.tutorialOn) {
                        tutorial.showNextStep()
                    }
                }*/

        setupViews()
    }

    private fun setupViews() {
        setupTouchControls()
        setupKeyboard()
        setupBackPressHandler()
        setupAudioControls()
        setupAudioObservers()
        setupDisplayControls()
        setupManualDisconnect()

        viewModel.connectionState.observe(viewLifecycleOwner) { isConnected ->
            if (!isConnected) {
                connectionLossHandler("Connection lost")
            }
        }
    }

    private fun setupConnectionMonitoring(delay: Int) {
        connectionMonitor = viewModel.connectionManager?.let { ConnectionMonitor.getInstance(it, viewModel) }
        connectionMonitor?.setHeartbeatTimeout(sharedPrefsHelper.getHeartbeatTimeout())
        connectionMonitor?.setHeartbeatInterval(sharedPrefsHelper.getHeartbeatInterval())
        connectionMonitor?.setHeartmeatMaxMissed(sharedPrefsHelper.getMaxMissedHeartbeats())
        connectionMonitor!!.startHeartbeat(delay)
    }

    private fun setupBackPressHandler() {
        val callback = object : OnBackPressedCallback(true) {
            override fun handleOnBackPressed() {
                val drawerLayout = requireActivity().findViewById<DrawerLayout>(R.id.drawer_layout)

                when {
                    drawerLayout.isDrawerOpen(GravityCompat.START) -> {
                        drawerLayout.closeDrawer(GravityCompat.START)
                    }

                    audioListVisible -> {
                        binding.audioRecyclerview.visibility = View.GONE
                        binding.expandArrowup.visibility = View.VISIBLE
                        binding.expandArrowdown.visibility = View.GONE
                        audioListVisible = false
                    }

                    binding.audioControlPanel.visibility == View.VISIBLE -> {
                        binding.audioControlPanel.visibility = View.GONE
                    }

                    displayListVisible -> {
                        binding.displayRecyclerview.visibility = View.GONE
                        displayListVisible = false
                    }

                    binding.editText.visibility != View.VISIBLE -> {
                        findNavController().navigate(R.id.action_to_main)
                    }

                    else -> {
                        findNavController().navigate(R.id.action_to_main)
                    }
                }
            }
        }

        // Add the callback to the activity's back press dispatcher
        requireActivity().onBackPressedDispatcher.addCallback(viewLifecycleOwner, callback)
    }

    @SuppressLint("ClickableViewAccessibility")
    private fun setupTouchControls() {
        val vibrator = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            val vibratorManager =
                requireContext().getSystemService(Context.VIBRATOR_MANAGER_SERVICE) as VibratorManager
            vibratorManager.defaultVibrator
        } else {
            @Suppress("DEPRECATION")
            requireContext().getSystemService(Context.VIBRATOR_SERVICE) as Vibrator
        }

        val gestureDetector =
            GestureDetector(requireContext(), object : GestureDetector.SimpleOnGestureListener() {
                override fun onSingleTapUp(e: MotionEvent): Boolean {
                    messageHost("MOUSE_LMB")
                    return true
                }

                override fun onDoubleTap(e: MotionEvent): Boolean {
                    messageHost("MOUSE_LMB")
                    return true
                }

                override fun onLongPress(e: MotionEvent) {
                    if (vibrator.hasVibrator()) {
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                            vibrator.vibrate(
                                VibrationEffect.createOneShot(
                                    10,
                                    VibrationEffect.DEFAULT_AMPLITUDE
                                )
                            )
                        } else {
                            @Suppress("DEPRECATION")
                            vibrator.vibrate(10)
                        }
                    }
                    messageHost("MOUSE_LMB_HOLD")
                }

                override fun onScroll(
                    e1: MotionEvent?,
                    e2: MotionEvent,
                    distanceX: Float,
                    distanceY: Float
                ): Boolean {
                    if (e2.pointerCount == 2) {
                        scrolling = true
                        messageHost("MOUSE_SCROLL $distanceY") // Negative for natural scrolling
                        return true
                    }
                    return false
                }
            })

        binding.touchpadFrame.setOnTouchListener { _, event ->
            gestureDetector.onTouchEvent(event)

            when (event.actionMasked) {
                MotionEvent.ACTION_DOWN -> {
                    startX = event.x
                    currentX = startX
                    startY = event.y
                    currentY = startY
                    true
                }

                MotionEvent.ACTION_MOVE -> {
                    x = event.x
                    y = event.y
                    deltaX = (x - currentX) * mouseSensitivity
                    deltaY = (y - currentY) * mouseSensitivity
                    if (!rmbDetected && !scrolling) {
                        messageHost("MOUSE_MOVE $deltaX $deltaY")
                        currentX = x
                        currentY = y
                    }
                    true
                }

                MotionEvent.ACTION_POINTER_DOWN -> {
                    currentX = event.x
                    currentY = event.y
                    startTime = System.currentTimeMillis()
                    initialPointerCount = event.pointerCount
                    scrolling = true
                    rmbDetected = true
                    true
                }

                MotionEvent.ACTION_POINTER_UP -> {
                    deltaT = (System.currentTimeMillis() - startTime).toFloat()
                    when {
                        initialPointerCount == 2 && deltaT in 10f..clickThreshold.toFloat() -> {
                            messageHost("MOUSE_RMB")
                            initialPointerCount = 0
                            true
                        }

                        initialPointerCount == 3 && deltaT in 10f..clickThreshold.toFloat() -> {
                            messageHost("MOUSE_MMB")
                            initialPointerCount = 0
                            true
                        }

                        event.pointerCount <= 1 -> {
                            scrolling = false // Reset when a finger is lifted
                            rmbDetected = false // Reset RMB detection when a finger is lifted
                            true
                        }

                        else -> true
                    }
                }

                MotionEvent.ACTION_UP -> {
                    scrolling = false // Reset when all fingers are lifted
                    rmbDetected = false // Reset when all fingers are lifted
                    true
                }

                else -> false
            }
        }
    }

    private fun setupAudioObservers() {
        viewModel.audioState.observe(viewLifecycleOwner) { state ->
            state?.let {
                if (it.error != null) {
                    Toast.makeText(requireContext(), it.error, Toast.LENGTH_SHORT).show()
                    if (it.error == "Connection lost") {
                        connectionLossHandler("Connection lost")
                    }
                    return@observe
                }

                // Update UI with new state
                audioRecyclerAdapter.updateOptions(it.devices)
                setAudioDeviceDefault(it.defaultDevice)

                binding.volumeSlider.progress = it.currentVolume
                binding.audioSliderVolume.text = it.currentVolume.toString()

                val (buttonColor, textColor) = if (it.isMuted) {
                    R.color.black to R.color.black
                } else {
                    R.color.grey to R.color.grey
                }

                binding.audioTogglemuteButton.setColorFilter(
                    ContextCompat.getColor(requireContext(), buttonColor)
                )
                binding.audioSliderVolume.setTextColor(
                    ContextCompat.getColor(requireContext(), textColor)
                )

                audioMuted = it.isMuted
            }
        }
    }

    private fun requestAudioDevices() {
        viewModel.requestAudioDevices()
    }

    private fun setAudioDeviceDefault(defaultDevice: String) {
        audioRecyclerAdapter.setSelectedPosition(defaultDevice)
    }

    private fun setupAudioControls() {
        // Initialize views using view binding
        binding.audiocycle.setOnClickListener {
            if (audioControlPanel.visibility == View.GONE) {
                hideDisplayProfilesList()
                audioControlPanel.visibility = View.VISIBLE
                try {
                    requestAudioDevices()
                } catch (e: SocketException) {
                    throw RuntimeException(e)
                }
            } else {
                hideAudioControlPanel()
            }
        }

        // Handle Expand Button Click
        binding.expandAudioButton.setOnClickListener {
            audioListVisible = !audioListVisible
            audioRecyclerViewOptions.visibility = if (audioListVisible) View.VISIBLE else View.GONE
            expandArrowUp.visibility = if (audioListVisible) View.GONE else View.VISIBLE
            expandArrowDown.visibility = if (audioListVisible) View.VISIBLE else View.GONE
        }

        binding.audioPlayPauseButton.setOnClickListener {
            messageHost("AUDIO TogglePlay")
        }

        binding.audioPreviousButton.setOnClickListener {
            messageHost("AUDIO PreviousTrack")
        }

        binding.audioNextButton.setOnClickListener {
            messageHost("AUDIO NextTrack")
        }

        binding.audioCycleButton.setOnClickListener {
            messageHost("AUDIO CycleDevices")
            cycleAudioDevice()
        }

        binding.audioTogglemuteButton.setOnClickListener {
            audioMuted = !audioMuted
            messageHost("AUDIO MUTE")

            val (buttonColor, textColor) = if (audioMuted) {
                R.color.black to R.color.black
            } else {
                R.color.grey to R.color.grey
            }

            binding.audioTogglemuteButton.setColorFilter(
                ContextCompat.getColor(
                    requireContext(),
                    buttonColor
                )
            )
            binding.audioSliderVolume.setTextColor(
                ContextCompat.getColor(
                    requireContext(),
                    textColor
                )
            )
        }

        binding.volumeSlider.setOnSeekBarChangeListener(object : OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar, progress: Int, fromUser: Boolean) {
                if (fromUser) {
                    messageHost("AudioVolume $progress")
                    binding.audioSliderVolume.text = progress.toString()
                }
            }

            override fun onStartTrackingTouch(seekBar: SeekBar) {
                // Optional: Add any pre-sliding actions
            }

            override fun onStopTrackingTouch(seekBar: SeekBar) {
                // Optional: Add any post-sliding actions
            }
        })
    }

    private fun setupKeyboard() {

        editText = binding.editText

        editText.apply {
            isFocusable = true
            isFocusableInTouchMode = true
            setOnFocusChangeListener { _, hasFocus ->
                if (hasFocus) {
                    val imm =
                        context.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
                    imm.showSoftInput(this, InputMethodManager.SHOW_IMPLICIT)
                }
            }
        }

        editText.setOnEditorActionListener { _, actionId, event ->
            if (actionId == EditorInfo.IME_ACTION_DONE ||
                (event?.keyCode == KeyEvent.KEYCODE_ENTER && event.action == KeyEvent.ACTION_DOWN)
            ) {
                if (!toolbar.getModifierToggled()) {
                    messageHost("KEYBOARD_WRITE {ENTER}")
                    toolbar.getKeyboardTextView().text = ""
                }
                true
            } else {
                false
            }
        }

        editText.onBackspacePressed = {
            val cursorPosition = editText.selectionStart
            if (cursorPosition > 0) {
                editText.text?.delete(cursorPosition - 1, cursorPosition)
                val textViewText = toolbar.getKeyboardTextView().text.toString()
                if (textViewText.isNotEmpty()) {
                    toolbar.getKeyboardTextView().text =
                        textViewText.substring(0, textViewText.length - 1)
                }
            }
            if (!toolbar.getModifierToggled()) {
                messageHost("KEYBOARD_WRITE {BS}")
            }
        }

        editText.addTextChangedListener(object : TextWatcher {
            override fun beforeTextChanged(s: CharSequence, start: Int, count: Int, after: Int) {
                // Empty
            }

            override fun onTextChanged(s: CharSequence, start: Int, before: Int, count: Int) {
                if (count > before) {
                    val addedChar = s[start + count - 1]
                    if (!toolbar.getModifierToggled()) {
                        messageHost("KEYBOARD_WRITE $addedChar")
                    } else {
                        toolbar.appendKeyCombination(addedChar)
                    }
                    toolbar.getKeyboardTextView().append(addedChar.toString())
                }
            }

            override fun afterTextChanged(s: Editable) {
                // Empty
            }
        })

        // Initialize row 2 and 3 button arrays for keyboardToolbar
        toolbar = KeyboardToolbar(
            binding.moreOpt,
            binding.winKey,
            binding.fnKey,
            binding.altKey,
            binding.ctrlKey,
            binding.shiftKey,
            binding.keyboardInputView,
            binding.keyboardToolbar,
            binding.keyboardExtraButtonsGrid,
            arrayOf(
                binding.insertKey,
                binding.deleteKey,
                binding.printScreenKey,
                binding.tabKey,
                binding.upKey,
                binding.escKey,
                binding.soundUpKey,
                binding.soundDownKey,
                binding.muteKey,
                binding.leftKey,
                binding.downKey,
                binding.rightKey
            ),
            arrayOf(
                binding.f1Key,
                binding.f2Key,
                binding.f3Key,
                binding.f4Key,
                binding.f5Key,
                binding.f6Key,
                binding.f7Key,
                binding.f8Key,
                binding.f9Key,
                binding.f10Key,
                binding.f11Key,
                binding.f12Key
            )
        )

        binding.moreOpt.setOnClickListener { //v: View? ->
            toolbar.keyboardExtraSetRowVisibility(
                toolbar.nextToolbarPage()
            )
        }

        // Set up click listeners for all toolbar buttons
        toolbar.setupButtonClickListeners { view ->
            extraToolbarOnClick(view)
        }

        // 2. Keyboard button click handler
        binding.keyboardImgBtn.setOnClickListener {
            hideAudioControlPanel()
            hideDisplayProfilesList()

            editText.run {
                visibility = View.VISIBLE
                isCursorVisible = false
                requestFocus()
            }
        }

        // Create and store the listener
        keyboardLayoutListener = ViewTreeObserver.OnGlobalLayoutListener {
            if (!isAdded || isRemoving) return@OnGlobalLayoutListener

            val rect = Rect()
            binding.root.getWindowVisibleDisplayFrame(rect)

            val screenHeight = binding.root.rootView.height
            val keypadHeight = screenHeight - rect.bottom

            val keyboardVisible = keypadHeight > screenHeight * 0.15

            if (keyboardVisible) {
                toolbar.toggleKeyboardToolbar(true)
                toolbar.getKeyboardTextView().visibility = View.VISIBLE
            } else {
                toolbar.toggleKeyboardToolbar(false)
                clearEditText()
                toolbar.getKeyboardTextView().text = ""
                toolbar.getKeyboardTextView().visibility = View.GONE
            }
        }

        // Add the listener
        binding.root.viewTreeObserver.addOnGlobalLayoutListener(keyboardLayoutListener)
    }

    private fun setupManualDisconnect() {
        binding.disconnectHost.setOnClickListener {
            if (viewModel.connectionManager?.getConnectionEstablished() == true) {
                viewModel.connectionManager?.shutdown()
            }
            connectionLossHandler("Disconnected from host")
        }
    }

    /*    private fun interactionTutorial() {
            interactionHelp.setOnClickListener {
                val builder = AlertDialog.Builder(this@InteractionPage)
                val tutorial = TutorialMediator.GetInstance(builder)
                if (interactionsHelpText.getVisibility() == View.VISIBLE) {
                    interactionsHelpText.setVisibility(View.GONE) // Hide the TextView
                } else {
                    interactionsHelpText.setVisibility(View.VISIBLE) // Show the TextView

                    if (startTutorial.getVisibility() != View.VISIBLE) {
                        startTutorial.setVisibility(View.VISIBLE) // Show clickable TextView for starting tutorial
                        startTutorial.setOnClickListener(View.OnClickListener { // Hide after clicked
                            startTutorial.setVisibility(View.GONE)

                            // Initiate tutorial starting at remote page steps
                            tutorial.tutorialOn = true
                            tutorial.currentStep = 0
                            val intent = Intent(this@InteractionPage, ServersPage::class.java)
                            startActivity(intent)
                            tutorial.showNextStep()
                        })
                    }

                    // Cancel any existing hide callbacks
                    handler.removeCallbacks(HideStartTutorial)
                    // Hide the button automatically after a delay
                    handler.postDelayed(HideStartTutorial, 2500)
                }
            }
        }*/

    private fun setupDisplayControls() {
        binding.displays.setOnClickListener {
            if (binding.displayRecyclerview.visibility == View.GONE) {
                hideAudioControlPanel()
                binding.displayRecyclerview.visibility = View.VISIBLE
                requestDisplayProfiles()
            } else {
                binding.displayRecyclerview.visibility = View.GONE
            }
        }

        viewModel.displayProfiles.observe(viewLifecycleOwner) { result ->
            when (result) {
                is com.example.dairemote_app.viewmodels.Result.Loading -> {
                    // Show loading indicator if needed
                }

                is com.example.dairemote_app.viewmodels.Result.Success -> {
                    updateDisplayProfiles(result.data)
                    displayListVisible = true
                }

                is com.example.dairemote_app.viewmodels.Result.Error -> {
                    if (result.message == "Connection lost") {
                        connectionLossHandler(result.message)
                    } else {
                        Toast.makeText(requireContext(), result.message, Toast.LENGTH_SHORT).show()
                        updateDisplayProfiles(listOf("Failed to load profiles"))
                    }
                }
            }
        }
    }

    private fun releaseVibrator() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            (requireContext().getSystemService(Context.VIBRATOR_MANAGER_SERVICE) as VibratorManager?)?.defaultVibrator?.cancel()
        } else {
            @Suppress("DEPRECATION")
            (requireContext().getSystemService(Context.VIBRATOR_SERVICE) as Vibrator?)?.cancel()
        }
    }

    private fun cleanUp() {
        keyboardLayoutListener?.let {
            binding.root.viewTreeObserver.removeOnGlobalLayoutListener(it)
        }
        keyboardLayoutListener = null

        releaseVibrator()

        viewModel.audioState.removeObservers(viewLifecycleOwner)
        viewModel.displayProfiles.removeObservers(viewLifecycleOwner)
        viewModel.connectionState.removeObservers(viewLifecycleOwner)
        viewModel.updateConnectionState(false)
        viewModel.connectionManager?.setConnectionEstablished(false)

        connectionMonitor?.shutDownHeartbeat()
        handler.removeCallbacksAndMessages(null)
        _binding = null
    }

    override fun onDestroyView() {
        cleanUp()

        super.onDestroyView()
    }

    private fun clearEditText() {
        // Hide the EditText
        editText.setText("")
        editText.visibility = View.GONE
        editText.clearFocus()
    }

    private fun extraToolbarOnClick(view: View) {
        val viewID = view.id
        var msg = ""
        var audio = false

        // Get all modifier button views
        val modifierButtons = listOf(
            binding.winKey,
            binding.fnKey,
            binding.altKey,
            binding.ctrlKey,
            binding.shiftKey
        )

        // Handle modifier buttons differently
        val isModifierButton = modifierButtons.any { it.id == viewID }

        if (isModifierButton) {
            // Check if this modifier was already active
            val wasAlreadyActive = when (viewID) {
                R.id.winKey -> toolbar.winActive
                R.id.fnKey -> toolbar.fnActive
                R.id.altKey -> toolbar.altActive
                R.id.ctrlKey -> toolbar.ctrlActive
                R.id.shiftKey -> toolbar.shiftActive
                else -> false
            }

            // If this modifier was already active, deactivate all
            if (wasAlreadyActive) {
                if (toolbar.getKeyCombination().isNotEmpty()) {
                    toolbar.addParentheses()
                    messageHost("KEYBOARD_WRITE ${toolbar.getKeyCombination()}")
                }

                resetKeyboardModifiers()
                return
            }

            // Toggle the modifier state
            toolbar.setModifierToggled(toolbar.keyboardToolbarModifier(viewID))

            // Update visual state for all modifiers
            updateModifierButtonColors()
            return
        }

        // Handle regular buttons
        resetKeyboardModifiers()

        // Find which button was pressed
        for (i in 0..11) {
            val buttons =
                if (toolbar.getCurrentToolbarPage() == 0) toolbar.getButtons(0) else toolbar.getButtons(
                    1
                )
            if (viewID == buttons[i].id) {
                if (toolbar.getCurrentToolbarPage() == 0 && (i == 6 || i == 7 || i == 8)) {
                    audio = true
                }
                msg =
                    if (toolbar.getCurrentToolbarPage() == 0) toolbar.getKeys(0)[i] else toolbar.getKeys(
                        1
                    )[i]
                break
            }
        }

        // Visual feedback
        view.setBackgroundColor(Color.LTGRAY)
        Handler().postDelayed({
            view.setBackgroundColor(Color.TRANSPARENT)
        }, 75)

        // Handle the action
        when {
            audio -> messageHost("AUDIO $msg")
            msg.isNotEmpty() -> messageHost("KEYBOARD_WRITE $msg")
        }
    }

    private fun updateModifierButtonColors() {
        binding.winKey.setBackgroundColor(if (toolbar.winActive) Color.LTGRAY else Color.TRANSPARENT)
        binding.fnKey.setBackgroundColor(if (toolbar.fnActive) Color.LTGRAY else Color.TRANSPARENT)
        binding.altKey.setBackgroundColor(if (toolbar.altActive) Color.LTGRAY else Color.TRANSPARENT)
        binding.ctrlKey.setBackgroundColor(if (toolbar.ctrlActive) Color.LTGRAY else Color.TRANSPARENT)
        binding.shiftKey.setBackgroundColor(if (toolbar.shiftActive) Color.LTGRAY else Color.TRANSPARENT)
    }

    private fun resetKeyboardModifiers() {
        toolbar.resetKeyboardModifiers()
        updateModifierButtonColors()
        editText.setText("")
    }

    override fun onStart() {
        super.onStart()
        setupConnectionMonitoring(250)
    }

    override fun onStop() {
        super.onStop()
        connectionMonitor?.shutDownHeartbeat()
    }

    private fun hideAudioControlPanel() {
        audioControlPanel.visibility = View.GONE
        audioRecyclerViewOptions.visibility = View.GONE
        expandArrowUp.visibility = View.VISIBLE
        expandArrowDown.visibility = View.GONE
        audioListVisible = false
    }

    private fun hideDisplayProfilesList() {
        if (displayListVisible) {
            displayRecyclerViewOptions.visibility = View.GONE
            displayListVisible = false
        }
    }

    private fun cycleAudioDevice() {
        audioRecyclerAdapter.cyclePosition()
    }

    private fun requestDisplayProfiles() {
        viewModel.requestDisplayProfiles()
    }

    private fun updateDisplayProfiles(profiles: List<String>) {
        displayProfilesRecyclerAdapter.updateOptions(profiles)
    }
}