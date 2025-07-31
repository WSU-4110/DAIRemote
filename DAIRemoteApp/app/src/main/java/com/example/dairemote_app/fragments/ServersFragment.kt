package com.example.dairemote_app.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.EditText
import android.widget.ImageView
import android.widget.Toast
import androidx.activity.OnBackPressedCallback
import androidx.appcompat.app.AlertDialog
import androidx.core.view.GravityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import com.example.dairemote_app.R
import com.example.dairemote_app.databinding.FragmentServersBinding
import com.example.dairemote_app.utils.ConnectionManager
import com.example.dairemote_app.utils.MdnsHostDiscovery
import com.example.dairemote_app.utils.SharedPrefsHelper
import com.example.dairemote_app.viewmodels.ConnectionViewModel
import com.google.android.material.floatingactionbutton.FloatingActionButton
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors

class ServersFragment : Fragment() {
    private var _binding: FragmentServersBinding? = null
    private val binding get() = _binding!!

    private val availableHosts = mutableListOf<String>()
    private lateinit var adapter: ArrayAdapter<String>
    lateinit var addServer: FloatingActionButton
    private lateinit var executor: ExecutorService
    private lateinit var viewModel: ConnectionViewModel
    private lateinit var sharedPrefsHelper: SharedPrefsHelper
    private var mdns: MdnsHostDiscovery? = null
    private var scope: CoroutineScope? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentServersBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // Initialize ViewModel
        viewModel = ViewModelProvider(requireActivity())[ConnectionViewModel::class.java]
        sharedPrefsHelper = SharedPrefsHelper(requireContext())

        setupBackPressHandler()
        setupViews()
        setupListeners()
        searchHosts()
    }

    private fun setupViews() {
        // Create a custom adapter to show stars for saved hosts
        adapter = object : ArrayAdapter<String>(
            requireContext(),
            R.layout.list_item_host,
            R.id.host_text_view,
            availableHosts
        ) {
            override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
                val view = super.getView(position, convertView, parent)
                val host = getItem(position)
                val savedHost = sharedPrefsHelper.getLastConnectedHost()

                // Find the star icon in your custom layout
                val starIcon = view.findViewById<ImageView>(R.id.star_icon)
                starIcon.visibility = if (host == savedHost) View.VISIBLE else View.GONE

                return view
            }
        }

        binding.hostList.adapter = adapter
        executor = Executors.newSingleThreadExecutor()

        // Setup swipe-to-refresh
        binding.swipeRefreshLayout.setOnRefreshListener {
            searchHosts()
        }
    }

    private fun setupListeners() {
        binding.hostList.setOnItemClickListener { _, _, position, _ ->
            attemptConnection(availableHosts[position])
        }

        // Long press to forget host
        binding.hostList.setOnItemLongClickListener { _, _, position, _ ->
            val host = availableHosts[position]
            val savedHost = sharedPrefsHelper.getLastConnectedHost()

            if (host == savedHost) {
                showForgetHostDialog(host)
            }

            true
        }

        binding.addServerBtn.setOnClickListener {
            showAddServerDialog()
        }
    }

    private fun showForgetHostDialog(host: String) {
        AlertDialog.Builder(requireContext())
            .setTitle("Forget Host")
            .setMessage("Do you want to forget $host?")
            .setPositiveButton("Forget") { _, _ ->
                sharedPrefsHelper.clearLastConnectedHost()
                adapter.notifyDataSetChanged() // Refresh the list to remove the star
                Toast.makeText(requireContext(), "Host forgotten", Toast.LENGTH_SHORT).show()
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun searchHosts() {
        binding.swipeRefreshLayout.isRefreshing = true

        viewModel.searchForHosts("Hello, DAIRemote").observe(viewLifecycleOwner) { result ->

            when (result) {
                is ConnectionViewModel.HostSearchResult.Success -> {
                    availableHosts.clear()
                    availableHosts.addAll(result.hosts)

                    if(availableHosts.isEmpty()) {
                        scope = CoroutineScope(Dispatchers.Main + SupervisorJob())
                        mdns = MdnsHostDiscovery(requireContext())
                        scope!!.launch {
                            withContext(Dispatchers.IO) {
                                // Run discovery off the main thread
                                mdns?.startDiscovery { discoveredHosts ->
                                    // Back on main thread to update UI
                                    scope!!.launch {
                                        withContext(Dispatchers.Main) {
                                            availableHosts.addAll(discoveredHosts)
                                        }
                                    }
                                }
                            }
                        }
                    }

                    requireActivity().runOnUiThread {
                        processAndDisplayHosts()
                    }
                }

                is ConnectionViewModel.HostSearchResult.Error -> {
                    startMdnsFallback()
                }
            }
        }
    }

    private fun startMdnsFallback() {
        val scope = CoroutineScope(Dispatchers.IO + SupervisorJob())
        val mdns = MdnsHostDiscovery(requireContext())

        scope.launch {
            withContext(Dispatchers.IO) {
                // Run discovery off the main thread
                mdns.startDiscovery { discoveredHosts ->
                    // Back on main thread to update UI
                    scope.launch {
                        withContext(Dispatchers.Main) {
                            if(discoveredHosts.isEmpty()) {
                                notifyUser("No hosts found")
                                binding.swipeRefreshLayout.isRefreshing = false
                            } else {
                                availableHosts.clear()
                                availableHosts.addAll(discoveredHosts)
                                processAndDisplayHosts()
                            }
                        }
                    }
                }
            }
        }
    }

    private fun processAndDisplayHosts() {
        // Highlight saved host if it exists
        sharedPrefsHelper.getLastConnectedHost()?.let { savedHost ->
            if (availableHosts.contains(savedHost)) {
                availableHosts.remove(savedHost)
                availableHosts.add(0, savedHost)
            }
        }

        adapter.notifyDataSetChanged()
        binding.swipeRefreshLayout.isRefreshing = false
    }

    private fun showAddServerDialog() {
        val inputField = EditText(requireContext()).apply {
            hint = "Enter IP Address here"
        }

        AlertDialog.Builder(requireContext())
            .setTitle("Add your server host here:")
            .setView(inputField)
            .setPositiveButton("Connect") { _, _ ->
                val host = inputField.text.toString().trim()
                if (host.isNotEmpty()) {
                    attemptConnection(host)
                } else {
                    notifyUser("Server IP cannot be empty")
                }
            }
            .setNegativeButton("Cancel") { dialog, _ -> dialog.dismiss() }
            .show()
    }

    private fun notifyUser(message: String) {
        Toast.makeText(requireContext(), message, Toast.LENGTH_SHORT).show()
    }

    private fun initiateInteractionPage(message: String) {
        notifyUser(message)
        findNavController().navigate(R.id.action_to_interaction)
    }

    private fun priorConnectionEstablishedCheck(host: String): Boolean {
        viewModel.connectionManager?.let { manager ->
            if (manager.getConnectionEstablished()) {
                if (host != viewModel.getSavedHost(requireContext())) {
                    // Stop the current connection before attempting a new one
                    manager.shutdown()
                    viewModel.updateConnectionState(false)
                    viewModel.connectionManager = null // Clear the old manager
                    return false // Allow new connection attempt
                } else {
                    initiateInteractionPage("Already connected")
                    return true
                }
            }
        }
        return false
    }

    private fun attemptConnection(server: String) {
        binding.connectionLoading.visibility = View.VISIBLE

        if (!priorConnectionEstablishedCheck(server)) {
            viewModel.connectionManager = ConnectionManager(server, viewModel)
            val manager = viewModel.connectionManager ?: run {
                binding.connectionLoading.visibility = View.GONE
                notifyUser("Connection manager initialization failed")
                return
            }

            executor.execute {
                try {
                    val connectionResult = manager.initializeConnection()

                    requireActivity().runOnUiThread {
                        binding.connectionLoading.visibility = View.GONE

                        if (connectionResult) {
                            // Save the successful connection
                            sharedPrefsHelper.saveLastConnectedHost(server)
                            adapter.notifyDataSetChanged() // Update star visibility
                            initiateInteractionPage("Connected to: $server")
                        } else {
                            notifyUser("Connection failed")
                            manager.resetConnectionManager()
                        }
                    }
                } catch (e: Exception) {
                    requireActivity().runOnUiThread {
                        binding.connectionLoading.visibility = View.GONE
                        notifyUser("Connection error: ${e.message}")
                        manager.resetConnectionManager()
                    }
                }
            }
        } else {
            binding.connectionLoading.visibility = View.GONE
        }
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
                        findNavController().navigate(R.id.action_to_main)
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
        if (!executor.isShutdown) {
            executor.shutdownNow()
        }
    }
}