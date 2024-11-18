package com.example.dairemote_app;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

public class DisplayProfilesRecyclerAdapter extends RecyclerView.Adapter<DisplayProfilesRecyclerAdapter.OptionViewHolder> {
    private List<String> displayProfiles;

    public DisplayProfilesRecyclerAdapter(List<String> displayProfiles) {
        this.displayProfiles = displayProfiles;
    }

    @NonNull
    @Override
    public OptionViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.audio_item, parent, false);
        return new OptionViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull OptionViewHolder holder, int position) {
        if ("No display profiles".equals(displayProfiles.get(position))) {
            holder.button.setText("No Display Profiles");
            holder.button.setEnabled(false);
            holder.button.setBackgroundColor(Color.GRAY);
            holder.button.setOnClickListener(null); // Remove click listener
        } else {
            holder.button.setText(displayProfiles.get(position));
            holder.button.setBackgroundColor(Color.parseColor("#077063"));
            holder.button.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    MainActivity.connectionManager.SendHostMessage("DisplayConnect " + holder.button.getText().toString());
                }
            });
        }
    }

    @Override
    public int getItemCount() {
        return displayProfiles.size();
    }

    public void updateOptions(List<String> newOptions) {
        displayProfiles = newOptions;
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
