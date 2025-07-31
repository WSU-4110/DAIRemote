package com.example.dairemote_app.utils

import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import androidx.recyclerview.widget.RecyclerView
import com.example.dairemote_app.R

class AudioRecyclerAdapter(
    private var audioDevices: List<String>,
    private val connectionManager: ConnectionManager
) :
    RecyclerView.Adapter<AudioRecyclerAdapter.OptionViewHolder>() {
    private var selectedPosition = -1

    fun setSelectedPosition(audioDevice: String) {
        val position = audioDevices.indexOf(audioDevice)
        if (position != -1) {
            val previousPosition = selectedPosition
            selectedPosition = position

            if (previousPosition != -1) {
                notifyItemChanged(previousPosition)
            }
            notifyItemChanged(selectedPosition)
        }
    }

    fun cyclePosition() {
        if (selectedPosition != -1) {
            val previousPosition = selectedPosition
            selectedPosition = (selectedPosition + 1) % audioDevices.size

            if (previousPosition != -1) {
                notifyItemChanged(previousPosition)
            }
            notifyItemChanged(selectedPosition)
        }
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OptionViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.interaction_list_item, parent, false)
        return OptionViewHolder(view)
    }

    override fun onBindViewHolder(holder: OptionViewHolder, position: Int) {
        if ("No audio devices" == audioDevices[position]) {
            holder.button.text = "No Audio Devices"
            holder.button.isEnabled = false
            holder.button.setBackgroundColor(Color.GRAY)
            holder.button.setOnClickListener(null) // Remove click listener
        } else {
            holder.button.text = audioDevices[position]

            if (position == selectedPosition) {
                holder.button.setBackgroundColor(Color.parseColor("#044a41"))
            } else {
                holder.button.setBackgroundColor(Color.parseColor("#077063"))
            }

            holder.button.setOnClickListener {
                connectionManager.sendHostMessage(
                    "AudioConnect " + holder.button.text.toString(),
                    ConnectionManager.getInetAddress()
                )
                val previousPosition = selectedPosition
                selectedPosition = holder.adapterPosition

                if (previousPosition != -1) {
                    notifyItemChanged(previousPosition)
                }
                notifyItemChanged(selectedPosition)
            }
        }
    }

    override fun getItemCount(): Int {
        return audioDevices.size
    }

    fun updateOptions(newOptions: List<String>) {
        audioDevices = newOptions
        notifyDataSetChanged()
    }

    class OptionViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        var button: Button = itemView.findViewById(R.id.audioDevicesButton)
    }
}
