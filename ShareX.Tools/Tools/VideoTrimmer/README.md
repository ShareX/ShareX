# Video trimmer

The tool uses the configured ShareX FFmpeg executable, with no player library or ffprobe requirement.

Preview strategy:

- Generate twelve evenly spaced stills using input-side seeks, one background job at a time. This avoids a complete decode of long recordings just to populate a filmstrip.
- While scrubbing, show the nearest cached still immediately. After 220 ms without a position change, refine the preview with an accurate input seek and one decoded frame. Cancel superseded work and serialize refinement jobs.
- Retain twelve overview images plus an LRU of at most 48 refined images, each limited to 640 × 360. There are at most two preview FFmpeg processes: the overview worker and the refinement worker. Images are released when changing input or closing the window; no preview files are written to disk.
- These are still previews, not real-time playback. The displayed frame is near the requested timestamp; seeking to EOF previews slightly before EOF.

Fast export keeps the source container and stream-copies the first non-cover video, all audio tracks, and subtitles. Input seeking preserves keyframe preroll, so output boundaries and duration can differ from the selection. This limitation is shown in the window. Precise export explicitly opts into H.264/AAC MP4 encoding and omits subtitles. A failed stream copy never silently falls back to encoding.

Output is staged in a unique file next to the chosen destination and moved into place only after FFmpeg succeeds. The source path is rejected as a destination. Cancellation kills and reaps the FFmpeg process and removes the staging file.