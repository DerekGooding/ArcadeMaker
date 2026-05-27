using ArcadeMaker.Core.Resources;
using Microsoft.Xna.Framework.Audio;

namespace ArcadeMaker.Engines.MonoGame.Core.Runtime;

internal class SoundEffectPlaybackInstance : ArcadeMaker.Core.Runtime.SoundPlaybackInstance<SoundEffectInstance>
{
    public override float Volume
    {
        get => Instance.Volume;
        set => Instance.Volume = value;
    }

    public override float Pan
    {
        get => Instance.Pan;
        set => Instance.Pan = value;
    }

    public override float Pitch
    {
        get => Instance.Pitch;
        set => Instance.Pitch = value;
    }

    internal SoundEffectPlaybackInstance(Sound sound, SoundEffectInstance instance) : base(sound, instance)
    {
    }
}