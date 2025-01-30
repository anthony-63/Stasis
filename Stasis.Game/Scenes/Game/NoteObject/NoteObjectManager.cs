using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObjectManager {
    public NoteObject[] OrderedNotes = [];

    int StartProcess = 0;

    public List<int> ToUpdateIndices = [];

    GameScene Game;

    public delegate void NoteEventHandler(int idx);
    public NoteEventHandler? Hit;
    public NoteEventHandler? Miss;

    public bool Skippable = false;

    public NoteObjectManager(GameScene game, Player.Player player) {
        Game = game;
        LoadNotes();
        Hit += player.Hit;
        Miss += player.Miss;
    }

    public void Update(Cursor cursor) {
        UpdateSkippable(Game.Music);
        UpdateNotes(Game.Music, cursor);
        UpdateRenderer(Game.NoteRenderer, Game.Music);
    }

    public void UpdateSkippable(SyncAudioPlayer? music) {
        Skippable = OrderedNotes[StartProcess].Time - (music?.Time ?? 0f) > 4f;
    }

    public void SkipToNote(ref SyncAudioPlayer music) {
        music?.Seek(OrderedNotes[StartProcess].Time - 2f);
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

                Hit?.Invoke(OrderedNotes[i].Index);
            }

            if(!OrderedNotes[i].Hit && !OrderedNotes[i].InHitWindow(music.Time, music.Speed)) {
                OrderedNotes[i].Processed = true;
                didHitreg = true;

                Miss?.Invoke(OrderedNotes[i].Index);
            }

            if(didHitreg && OrderedNotes[i].Index < OrderedNotes.Length - 1) {
                StartProcess++;
            }
        }
    }

    void LoadNotes() {
        Logger.Info("Started Note Loading");
        OrderedNotes = new NoteObject[Global.SelectedMap?.Difficulties[0].Notes.Length ?? 1];

        for(int i = 0; i < (Global.SelectedMap?.Difficulties[0].Notes.Length ?? 0); i++) {
            var noteData = Global.SelectedMap?.Difficulties[0].Notes[i] ?? new Content.Beatmaps.Note();
            OrderedNotes[i] = new NoteObject(noteData, i, Global.Colors[i % Global.Colors.Length]);
        }

        Logger.Info($"Loaded {OrderedNotes.Length} Notes");
    }
}