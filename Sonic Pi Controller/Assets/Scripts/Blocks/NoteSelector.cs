using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSelector : MonoBehaviour
{
    void Start()
    {
        octave_ = 4;
    }

    public void UpOctave()
    {
        if(octave_ < 8) octave_++;
    }

    public void DownOctave()
    {
        if(octave_ > 1) octave_--;
    }

    public int GetOctave()
    {
        return octave_;
    }

    int octave_;
}
