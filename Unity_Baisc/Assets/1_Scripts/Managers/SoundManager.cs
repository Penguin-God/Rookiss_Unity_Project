using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    Dictionary<string, AudioClip> audioClipByPath = new Dictionary<string, AudioClip>(); 

    public void Init()
    {
        GameObject _root = GameObject.Find("@Sound");
        if (_root == null)
        {
            _root = new GameObject("@Sound");
            Object.DontDestroyOnLoad(_root);

            string[] _soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < _soundNames.Length - 1; i++) // MaxCount 때문에 -1 해줌
            {
                GameObject go = new GameObject(_soundNames[i]);
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.SetParent(_root.transform);
            }

            audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        ClearAudioSources();
        audioClipByPath.Clear();
    }

    void ClearAudioSources()
    {
        foreach (var _sounce in audioSources)
        {
            _sounce.Stop();
            _sounce.clip = null;
        }
    }

    public void Play(string _path) => Play(LoadClip(_path));
    public void Play(AudioClip _clip) => audioSources[(int)Define.Sound.Effect].PlayOneShot(_clip);

    public void PlayBgm(string _path) => PlayBgm(LoadClip(_path));
    public void PlayBgm(AudioClip _clip)
    {
        AudioSource _audioSource = audioSources[(int)Define.Sound.Bgm];
        if (_audioSource.isPlaying) _audioSource.Stop();

        _audioSource.clip = _clip;
        _audioSource.Play();
    }

    AudioClip LoadClip(string _path)
    {
        _path = SetPath(_path);
        if(audioClipByPath.TryGetValue(_path, out AudioClip _clip) == false)
        {
            _clip = Managers.Resources.Load<AudioClip>(_path);
            Debug.Assert(_clip != null, $"오디오 클립을 찾지 못함. 경로 : {_path}");
            audioClipByPath.Add(_path, _clip);
        }

        return _clip;
    }

    string SetPath(string _path)
    {
        if (!_path.Contains("Sounds/")) 
            _path = $"Sounds/{_path}";
        return _path;
    }
}
