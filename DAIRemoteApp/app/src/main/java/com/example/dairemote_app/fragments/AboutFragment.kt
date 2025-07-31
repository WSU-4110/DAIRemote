package com.example.dairemote_app.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.OnBackPressedCallback
import androidx.core.view.GravityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import com.example.dairemote_app.R
import com.example.dairemote_app.databinding.FragmentAboutBinding
import com.example.dairemote_app.viewmodels.ConnectionViewModel

class AboutFragment : Fragment() {
    private var binding: FragmentAboutBinding? = null
    private lateinit var viewModel: ConnectionViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        viewModel = ViewModelProvider(requireActivity())[ConnectionViewModel::class.java]
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        binding = FragmentAboutBinding.inflate(inflater, container, false)
        return binding!!.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        setupBackPressHandler()
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
                        // Let default back press behavior work
                        isEnabled = false
                        findNavController().navigate(R.id.action_to_main)
                    }
                }
            }
        }

        // Add the callback to the activity's back press dispatcher
        requireActivity().onBackPressedDispatcher.addCallback(viewLifecycleOwner, callback)
    }
}