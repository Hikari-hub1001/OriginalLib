使い方

プラットフォームごとに変更したいUIがある場合に使用できます。

１．シーン内の任意のオブジェクトに「PlatformOverriderGroup」をアタッチします。
　　シングルトンとなっています、シーン内に一つのみ配置可能です。
　　※未実行時に限り２つ以上配置できますが、実行直後に１つになるようにDestroyされます。
２．座標や大きさを変えたいオブジェクトに「PlatformOverrider」をアタッチします。
　　変更したいオブジェクトごとに1つ必要です。
３．任意のプラットフォームを選択し、デザインの調整を行ってください。
　　また、WindowsとMac、PS4、PS5は共通で、スマホの横だけ別デザインがいい
　　などという際はDefaultを設定し、それぞれのプラットフォームでUseDefaultにチェックを入れてください。

Simulatorモードで実行した場合はスマホモードになります。
Gameモードでの実行では自由に切り替え可能です。


How to Use:

If you want to make changes to your UI for different platforms, you can use this system.

1.Attach "PlatformOverriderGroup" to any object within your scene.
  It is a singleton, so you can place only one in your scene.
  Note: You can place more than one before runtime, but they will be reduced to one after runtime.

2.Attach "PlatformOverrider" to the objects for which you want to change the position or size.
  You need one "PlatformOverrider" component for each object you want to modify.

3.Choose the desired platform and adjust the design accordingly.
  For example, if you want the design to be the same for Windows, Mac, PS4, and PS5 but different for smartphones when in landscape mode, set it as "Default" and check "UseDefault" for each platform.

If you run it in Simulator mode, it will emulate a smartphone mode.
In Game mode, you can freely switch between platforms.