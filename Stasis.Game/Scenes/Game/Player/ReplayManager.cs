using Stasis.Content.Replays;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Engine.GFX;

namespace Stasis.Game.Scenes.Game.Player;

public class ReplayManager(Replay replay) {
    public Replay Replay = replay;
    int frameIndex = 0;

    double frameTimer = 0;

    public const int REPLAY_FRAME_PER_SECOND = 144;
    double replaySecPerFrame = 1.0/REPLAY_FRAME_PER_SECOND;

    public ReplayManager() : this(new Replay()) {}

    public void Save(string scoreDir, byte[] scoreHash, SyncAudioPlayer? music, Score score) {
        Replay.SaveFrame(new(), music?.Time ?? 0, score.Failed);

        var basePath = scoreDir + "/" + Util.GetSHA256(Global.SelectedMap?.Title + string.Concat(Replay.Frames.Select(x => x.CursorPosition.X + x.CursorPosition.Y + x.Time) ?? []));
        var time = DateTime.Now.ToFileTime();
        var replayPath = basePath + "/" + time.ToString() + ".sr";
        Replay.Export(replayPath, scoreHash);
    }

    public void PlayFrame(Cursor cursor, SyncAudioPlayer? music, Player player, Sprite grid) {
        if(Replay.Frames.Count < frameIndex) return;
        
        var currentFrame = Replay.Frames[frameIndex];
        if(currentFrame.Meta == ReplayFrameMeta.HIT) {
            player.Hit(0);
            frameIndex++;
        }
        else if(currentFrame.Meta == ReplayFrameMeta.MISS) {
            player.Miss(0);
            frameIndex++;
        }
        else if(currentFrame.Meta == ReplayFrameMeta.FAILED) player.Score.Failed = true;
        else {
            cursor.Position = currentFrame.CursorPosition;
            cursor.ClampedPosition = currentFrame.CursorPosition;
            cursor.ApplyParallax(player.Camera, grid);
        } 

        if((music?.Time ?? 0) > currentFrame.Time) {
            frameIndex++;
        }
    }

    public void UpdateFrameMaker(double dt,Cursor cursor, SyncAudioPlayer? music, Score score) {
        frameTimer += dt;
        if(frameTimer > replaySecPerFrame) {
            MakeFrame(cursor, music, score);
            frameTimer = 0;
        }
    }

    public void MakeFrame(Cursor cursor, SyncAudioPlayer? music, Score score) {
        Replay.SaveFrame(cursor.Position, music?.Time ?? 0, score.Failed);
    }
}