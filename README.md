<h1>Project Overview</h1>
<p>The laptop power managment system (LPM) is an automated system to manage laptop battery health by controlling the charging process via a relay, activated by an ESP32 microcontroller. The system features two operational modes: the first is a battery-preserving mode, where users can set a minimum and maximum battery percentage. In this mode, the system disconnects the charger when the battery reaches the maximum threshold and reconnects it when the battery discharges to the minimum level, thereby optimizing battery lifespan. The second mode allows for traditional charging to a specific battery percentage without the dynamic adjustment of the battery levels. Additionally, an override switch is included to bypass the automated settings, enabling continuous charging regardless of battery level and selected mode for scenarios requiring constant power. This setup aims to combine advanced battery management with user flexibility for maintaining laptop battery longevity and adapting to different usage needs.</p>
<h1>Detailed Specifiactions</h1>
<h2>Diagram</h2>
<img src="https://github.com/user-attachments/assets/7ac357d4-c5b9-487e-950b-b74b26b4139f">
<h2>Components</h2>
<ul>
  <li>ESP 32</li>
  <li>2x Electrolytic Capacitor 100µF</li>
  <li>2x Ceramic Capacitor 100nF</li>
  <li>7805 Voltage Regulator to 5v</li>
  <li>5v relay that can handle up to 20v DC 4 amps</li>
  <li>Laptop charger you are willing to cut (destroy)</li>
</ul>
<h1>How to Setup</h1>
<ol>
  <li>Connect Everything</li>
  <li>Go to the esp32 code (ESPApp/EspApp/EspApp.ino)</li>
  <li>Enter your wifi credentials and setup the static ip config</li>
  <li>Change the pins configuration if needed</li>
  <li>(optional) Generate a token which has to be present in every request made to the esp32</li>
  <li>Compile the windows forms app and run it</li>
  <li>Enter your server settings in the server settings tab</li>
</ol>
