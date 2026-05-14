## How to Run

Start with "Loadfirst" scene and press play on the editor. "Mr. Brightside" song is pre-loaded and has its configurations (bpm, interval, tile speed,...) initiated.  Once the pooling and spawning instance are both initialized, the game starts.

## Design Choices

- Due to time restrictions, I had to do a pre-load song instead of choosing from a list (if had time and resources, I could implement an asset bundle oriented solution)
- Decided to spawn tiles by elapsedTime and wait for interval to incorporate audio delay

## AI Usage

- **Claude**: Helped with requirement specifications clarity and planned task accordingly. Also helped with debugging unoptimized assets
- **Cursor**: Helped scaffold basic architecture and essentials scripts (in which worked initially but tinkering was done afterwards). Also helped with code review

## Asset Attributions

- **Particle Effect For UGU**: [https://github.com/mob-sakai/ParticleEffectForUGUI](https://github.com/mob-sakai/ParticleEffectForUGUI)
- **Cursor for Unity**: [https://github.com/boxqkrtm/com.unity.ide.cursor](https://github.com/boxqkrtm/com.unity.ide.cursor)
- **Hyper Casual FX**: [https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333](https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333)
- **Mobile Haptic Feedback**: [https://assetstore.unity.com/packages/tools/integration/mobile-haptic-feedback-320664](https://assetstore.unity.com/packages/tools/integration/mobile-haptic-feedback-320664)
- **DOTween**: [https://dotween.demigiant.com/index.php](https://dotween.demigiant.com/index.php)
- **Procedural UI Image**: [https://assetstore.unity.com/packages/tools/gui/procedural-ui-image-52200](https://assetstore.unity.com/packages/tools/gui/procedural-ui-image-52200)
- **Kenney**: [https://kenney.nl/](https://kenney.nl/)

## Known Issues:

- Configuration is not dynamic
- Some magic numbers were used

