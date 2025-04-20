using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObjectManager {
    public NoteObject[] OrderedNotes = [];

    public int StartProcess = 0;

    public List<int> ToUpdateIndices = [];

    NoteObject? NextNote = null;

    GameScene Game;

    public delegate void NoteEventHandler(float time);
    public NoteEventHandler? Hit;
    public NoteEventHandler? Miss;

    public bool Skippable = false;

    public NoteObjectManager(GameScene game, Player.Player player, float startFrom) {
        Game = game;
        LoadNotes(startFrom);
        if(!Global.Mods.VisualMap && Global.Replay is null) {
            Hit += player.Hit;
            Miss += player.Miss;
        }
    }

    public void Update(Cursor cursor) {
        UpdateSkippable(Game.Music);
        UpdateNotes(Game.Music, cursor);
        UpdateRenderer(Game.NoteRenderer, Game.Music);
    }

    public void UpdateSkippable(SyncAudioPlayer? music) {
        Skippable = NextNote?.Time - (music?.Time ?? 0f) > 4f;
    }

    public void SkipToNote(SyncAudioPlayer music) {
        music?.Seek(Math.Max((NextNote?.Time ?? 0) - 2f, 0));
        Logger.Info(music?.Time ?? 0);
    }

    public void UpdateRenderer(NoteObjectRenderer? renderer, SyncAudioPlayer? music) {
        if(renderer == null || music == null) return;
        renderer.ToRender.Clear();
        for(int i = StartProcess; i < OrderedNotes.Length; i++) {
            var note = OrderedNotes[i];
            if(note.IsVisible(music.Time, music.Speed, Global.Settings.Note.ApproachTime, Global.Settings.Note.Pushback))
                renderer.ToRender.Add(note);
            if(note.Time > music.Time + Global.Settings.Note.ApproachTime * music.Speed) break;
        }
    }

    public void UpdateNotes(SyncAudioPlayer? music, Cursor cursor) {
        if(music == null) return;

        ToUpdateIndices.Clear();

        for(int i = StartProcess; i < OrderedNotes.Length; i++) {
            var note = OrderedNotes[i];
            if(note.CalculateTime(music.Time, music.Speed, Global.Settings.Note.ApproachTime * music.Speed) <= 0 && !note.Hit)
                ToUpdateIndices.Add(i);
            if(note.Time > music.Time + Global.Settings.Note.ApproachTime * music.Speed) break;
        }

        foreach(int i in ToUpdateIndices) {
            var didHitreg = false;

            if(OrderedNotes[i].IsHitting(cursor.Position)) {
                OrderedNotes[i].Hit = true;
                OrderedNotes[i].Processed = true;
                didHitreg = true;

                Hit?.Invoke(music.Time);
            }

            if(!OrderedNotes[i].Hit && !OrderedNotes[i].InHitWindow(music.Time, music.Speed)) {
                OrderedNotes[i].Processed = true;
                didHitreg = true;

                Miss?.Invoke(music.Time);
            }

            if(didHitreg && OrderedNotes[i].Index < OrderedNotes.Length - 1) {
                NextNote = OrderedNotes[i+1];
                StartProcess++;
            }
        }
    }

    void LoadNotes(float startFrom) {
        Logger.Info("Started Note Loading");
        OrderedNotes = new NoteObject[Global.SelectedMap?.Difficulties[0].Notes.Length ?? 1];

        for(int i = 0; i < (Global.SelectedMap?.Difficulties[0].Notes.Length ?? 0); i++) {
            var noteData = Global.SelectedMap?.Difficulties[0].Notes[i] ?? new Note();
            var color = Raylib.GetColor((Global.Settings.Note.Colors[i % Global.Settings.Note.Colors.Length] << 8) | 0xff);
            OrderedNotes[i] = new NoteObject(noteData, i, color);
            if(noteData.Time <= startFrom) StartProcess += 1;
        }
        
        NextNote = OrderedNotes[StartProcess];

        Logger.Info($"Loaded {OrderedNotes.Length} Notes");
    }
}