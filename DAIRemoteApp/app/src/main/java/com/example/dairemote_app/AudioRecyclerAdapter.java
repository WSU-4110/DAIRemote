package com.example.dairemote_app;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

public class AudioRecyclerAdapter extends RecyclerView.Adapter<AudioRecyclerAdapter.OptionViewHolder> {
    private List<String> audioDevices;
    private int selectedPosition = -1;

    public AudioRecyclerAdapter(List<String> audioDevices) {
        this.audioDevices = audioDevices;
    }

    public void SetSelectedPosition(String audioDevice) {
        int position = audioDevices.indexOf(audioDevice);
        if (position != -1) {
            int previousPosition = selectedPosition;
            selectedPosition = position;

            if (previousPosition != -1) {
                notifyItemChanged(previousPosition);
            }
            notifyItemChanged(selectedPosition);
        }
    }

    public void CyclePosition() {
        if (selectedPosition != -1) {
            int previousPosition = selectedPosition;
            selectedPosition = (selectedPosition+1)%audioDevices.size();

            if (previousPosition != -1) {
                notifyItemChanged(previousPosition);
            }
            notifyItemChanged(selectedPosition);
        }
    }

    @NonNull
    @Override
    public OptionViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.audio_item, parent, false);
        return new OptionViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull OptionViewHolder holder, int position) {
        if ("No audio devices".equals(audioDevices.get(position))) {
            holder.button.setText("No Audio Devices");
            holder.button.setEnabled(false);
            holder.button.setBackgroundColor(Color.GRAY);
            holder.button.setOnClickListener(null); // Remove click listener
        } else {
            holder.button.setText(audioDevices.get(position));

            if (position == selectedPosition) {
                holder.button.setBackgroundColor(Color.parseColor("#044a41"));
            } else {
                holder.button.setBackgroundColor(Color.parseColor("#077063"));
            }

            holder.button.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    MainActivity.connectionManager.SendHostMessage("AudioConnect " + holder.button.getText().toString());
                    int previousPosition = selectedPosition;
                    selectedPosition = holder.getAdapterPosition();

                    if (previousPosition != -1) {
                        notifyItemChanged(previousPosition);
                    }
                    notifyItemChanged(selectedPosition);
                }
            });
        }
    }

    @Override
    public int getItemCount() {
        return audioDevices.size();
    }

    public void updateOptions(List<String> newOptions) {
        audioDevices = newOptions;
        notifyDataSetChanged();
    }

    public static class OptionViewHolder extends RecyclerView.ViewHolder {
        Button button;

        public OptionViewHolder(@NonNull View itemView) {
            super(itemView);
            button = itemView.findViewById(R.id.audioDevicesButton);
        }
    }
}
