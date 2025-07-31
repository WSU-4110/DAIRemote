package com.example.dairemote_app.fragments

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.activity.OnBackPressedCallback
import androidx.appcompat.app.AlertDialog
import androidx.core.view.GravityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.navigation.fragment.findNavController
import com.example.dairemote_app.R
import com.example.dairemote_app.databinding.FragmentMainBinding
import com.example.dairemote_app.utils.ConnectionManager
import com.example.dairemote_app.utils.MdnsHostDiscovery
import com.example.dairemote_app.utils.SharedPrefsHelper
import com.example.dairemote_app.utils.TutorialMediator
import com.example.dairemote_app.viewmodels.ConnectionViewModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class MainFragment : Fragment() {
    private var _binding: FragmentMainBinding? = null
    private val binding get() = _binding!!
    private val viewModel: ConnectionViewModel by activityViewModels()
    private lateinit var sharedPrefsHelper: SharedPrefsHelper
    private var mdns: MdnsHostDiscovery? = null
    private var scope: CoroutineScope? = null
    private val tutorial by lazy {
        TutorialMediator.GetInstance(AlertDialog.Builder(requireContext()))
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentMainBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        sharedPrefsHelper = SharedPrefsHelper(requireContext())

        // Check for saved host and attempt automatic connection
        val savedHost = sharedPrefsHelper.getLastConnectedHost()
        if (savedHost != null && viewModel.connectionState.value != true) {
            attemptConnection()
        }

        setupBackPressHandler()

        // Handle click listeners
        binding.DAIRemoteLogoBtn.setOnClickListener {
            val isConnected = viewModel.connectionState.value ?: false
            if (!isConnected) {
                animateConnectionButton(it)
                attemptConnection()
            } else {
                findNavController().navigate(R.id.action_to_interaction)
            }
        }
    }

    private fun animateConnectionButton(view: View) {
        view.animate().scaleX(1.2f).scaleY(1.2f).setDuration(150)
            .withEndAction {
                view.animate().scaleX(1f).scaleY(1f).setDuration(150).start()
            }.start()
    }

    private fun attemptConnection() {
        binding.connectionLoading.visibility = View.VISIBLE
        viewModel.searchForHosts().observe(viewLifecycleOwner) { result ->

            when (result) {
                is ConnectionViewModel.HostSearchResult.Success -> {
                    binding.connectionLoading.visibility = View.GONE
                    handleHostSearchResult(result.hosts)
                }

                is ConnectionViewModel.HostSearchResult.Error -> {
                    startMdnsFallback()
                }
            }
        }
    }

    private fun startMdnsFallback() {
        scope = CoroutineScope(Dispatchers.Main + SupervisorJob())
        mdns = MdnsHostDiscovery(requireContext())
        scope!!.launch {
            // Start discovery with 5 second timeout
            mdns?.startDiscovery(timeoutMillis = 5000L) { discoveredHosts ->
                binding.connectionLoading.visibility = View.GONE
                if(discoveredHosts.isEmpty()) {
                    notifyUser("No hosts found")
                } else {
                    handleHostSearchResult(discoveredHosts)
                }
            }
        }
    }

    private fun handleHostSearchResult(hosts: List<String>) {
        when {
            hosts.size == 1 -> {
                // Only one host available - connect automatically
                connectToHost(hosts[0])
            }

            else -> {
                // Multiple hosts available - check for previous connection
                val lastConnectedHost = sharedPrefsHelper.getLastConnectedHost()
                if (lastConnectedHost != null && hosts.contains(lastConnectedHost)) {
                    // Previous host found in list - connect automatically
                    connectToHost(lastConnectedHost)
                } else {
                    // No previous host or not in list - show selection
                    showHostSelectionDialog(hosts)
                }
            }
        }
    }

    private fun connectToHost(hostAddress: String) {
        viewModel.connectionManager = ConnectionManager(hostAddress, viewModel)
        sharedPrefsHelper.saveLastConnectedHost(hostAddress)

        viewModel.connectionManager.let { manager ->
            CoroutineScope(Dispatchers.IO).launch {
                val connectionResult = manager?.initializeConnection()

                withContext(Dispatchers.Main) {
                    if (connectionResult != null) {
                        viewModel.updateConnectionState(connectionResult)
                        manager.setConnectionEstablished(connectionResult)
                    }

                    if (connectionResult == true) {
                        notifyUser("Connection approved")
                        if (isAdded && !isDetached) {
//                            ActivityMainBinding.inflate(layoutInflater).HostName.text = "ZZZ"
                            findNavController().navigate(R.id.action_to_interaction)
                        }
                    } else {
                        notifyUser("Denied connection")
                        sharedPrefsHelper.clearLastConnectedHost()
                        manager?.resetConnectionManager()
                    }
                }
            }
        }
    }

    private fun showHostSelectionDialog(hosts: List<String>) {
        AlertDialog.Builder(requireContext())
            .setTitle("Select a host")
            .setItems(hosts.toTypedArray()) { _, which ->
                connectToHost(hosts[which])
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun notifyUser(message: String) {
        Toast.makeText(requireContext(), message, Toast.LENGTH_SHORT).show()
    }

    fun moveAppToBackground() {
        val intent = Intent(Intent.ACTION_MAIN)
        intent.addCategory(Intent.CATEGORY_HOME)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
        startActivity(intent)
    }

    private fun setupBackPressHandler() {
        val callback = object : OnBackPressedCallback(true) {
            override fun handleOnBackPressed() {
                val drawerLayout = requireActivity().findViewById<DrawerLayout>(R.id.drawer_layout)

                when {
                    drawerLayout.isDrawerOpen(GravityCompat.START) -> {
                        drawerLayout.closeDrawer(GravityCompat.START)
                    }

                    else -> {
                        moveAppToBackground()
                    }
                }
            }
        }

        // Add the callback to the activity's back press dispatcher
        requireActivity().onBackPressedDispatcher.addCallback(viewLifecycleOwner, callback)
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    override fun onDestroy() {
        super.onDestroy()
        mdns?.destroy()
    }
}