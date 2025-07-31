package com.example.dairemote_app.utils

import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import androidx.recyclerview.widget.RecyclerView
import com.example.dairemote_app.R

class DisplayProfilesRecyclerAdapter(
    private var displayProfiles: List<String>,
    private val connectionManager: ConnectionManager
) : RecyclerView.Adapter<DisplayProfilesRecyclerAdapter.OptionViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OptionViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.interaction_list_item, parent, false)
        return OptionViewHolder(view)
    }

    override fun onBindViewHolder(holder: OptionViewHolder, position: Int) {
        if ("No display profiles" == displayProfiles[position]) {
            holder.button.text = "No Display Profiles"
            holder.button.isEnabled = false
            holder.button.setBackgroundColor(Color.GRAY)
            holder.button.setOnClickListener(null) // Remove click listener
        } else {
            holder.button.text = displayProfiles[position]
            holder.button.setBackgroundColor(Color.parseColor("#077063"))
            holder.button.setOnClickListener { connectionManager.sendHostMessage("DisplayConnect " + holder.button.text.toString()) }
        }
    }

    override fun getItemCount(): Int {
        return displayProfiles.size
    }

    fun updateOptions(newOptions: List<String>) {
        displayProfiles = newOptions
        notifyDataSetChanged()
    }

    class OptionViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        var button: Button = itemView.findViewById(R.id.audioDevicesButton)
    }
}
