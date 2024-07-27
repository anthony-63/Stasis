using System.Runtime.InteropServices;
using Raylib_cs;
using Rhythia.Engine;
using Rhythia.Engine.Audio;
using Rhythia.Game.Scenes.Game.Player;

namespace Rhythia.Game.Scenes.Game.NoteObject;

public class NoteObjectSpawner {
    public NoteObject[] OrderedNotes = [];
    
    NoteObject? NextNote = null;
    NoteObject? LastNote = null;

    // int SkippedNotes = 0;
    int StartProcess = 0;

    public List<int> ToUpdateIndices = [];

    GameScene Game;

    public delegate void NoteEventHandler(int idx);
    public NoteEventHandler? Hit;
    public NoteEventHandler? Miss;

    public NoteObjectSpawner(GameScene game, Player.Player player) {
        Game = game;
        LoadNotes();
        Hit += player.Hit;
        Miss += player.Miss;
    }

    public void Update(Cursor cursor) {
        UpdateNotes(Game.Music, cursor);
        UpdateRenderer(Game.Renderer, Game.Music);
    }

    public int CalculateMaxInstanceCount() {
        float lifetime = Global.Settings.Note.ApproachTime + NoteObject.HitWindow;
        int max = 0;
        for(int i = 0; i < OrderedNotes.Length; i++) {
            int count = 0;
            for(int j = i; j < OrderedNotes.Length; j++) {
                if(OrderedNotes[j].Time > OrderedNotes[i].Time + lifetime) break;
                count++;
            }
            if(count > max) max = count;
        }
        Logger.Info("Max instances to be rendered: ", max);
        return max;
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
            if(note.CalculateTime(music.Time, Global.Settings.Note.ApproachTime * music.Speed) <= 0 && !note.Hit)
                ToUpdateIndices.Add(i);
            if(note.Time > music.Time + Global.Settings.Note.ApproachTime * music.Speed) break;
        }

        foreach(int i in ToUpdateIndices) {
            var didHitreg = false;

            if(OrderedNotes[i].IsHitting(cursor.Position)) {
                OrderedNotes[i].Hit = true;
                didHitreg = true;

                Hit?.Invoke(OrderedNotes[i].Index);
            }

            if(!OrderedNotes[i].Hit && !OrderedNotes[i].InHitWindow(music.Time, music.Speed)) {
                OrderedNotes[i].Hit = true;
                didHitreg = true;

                Miss?.Invoke(OrderedNotes[i].Index);
            }

            if(didHitreg) {
                LastNote = OrderedNotes[i];

                if(OrderedNotes[i].Index < OrderedNotes.Length - 1) {
                    NextNote = OrderedNotes[OrderedNotes[i].Index + 1];
                    StartProcess++;
                }
                else NextNote = null;
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