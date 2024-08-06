#include <WiFi.h>
#include <WebServer.h>
#include <ArduinoJson.h>

#define switchPin 4  //togle override button
//#define switchPin 15

#define relayPin  6
//#define switchPin 26

bool relayState = LOW;   
bool lastSwitchState = LOW; 

//optional - you can leave it empty
const char *authToken="";

const char *ssid = "";
const char *password = "";

IPAddress staticIP(192, 168, 0, 3);  // Set the desired static IP address
IPAddress gateway(192, 168, 0, 1);
IPAddress subnet(255, 255, 255, 0);
IPAddress dns(8, 8, 8, 8);

WebServer server(80);

const char *headerKeys[1] = {"token"};

bool waitingForDischarge=false;

unsigned long currentMillis = 0;

void setup() {
  server.collectHeaders(headerKeys, 1);
  Serial.begin(115200);
  
  WiFi.begin(ssid, password);
  WiFi.config(staticIP, gateway, subnet, dns);

  server.on("/", HTTP_GET, []() {
    if (strcmp(server.header("token").c_str(), authToken)!=0) {
    Serial.println("Invalid token");
    Serial.println(server.header("token"));
    server.send(403);
    return;
    }
    server.send(200,"plain/text","ESP Power Manager - status OK");
  });

  server.on("/data", HTTP_POST, [&]() {
    currentMillis=millis();

    if (strcmp(server.header("token").c_str(), authToken)!=0) {
    Serial.println("Invalid token");
    Serial.println(server.header("token"));
    server.send(403);
    return;
    }

    String body = server.arg("plain");
    Serial.println("Procesing new /data");
    Serial.println(body);

    JsonDocument parsedObj;
    DeserializationError error = deserializeJson(parsedObj, body);
    
    if (error) {
      Serial.print("deserializeJson() failed: ");
      Serial.println(error.f_str());
      server.send(400, "application/json", "{\"status\":\"error\",\"message\":\"Invalid JSON\"}");
      return;
    }
    serializeJsonPretty(parsedObj, Serial);

    bool isCharging=parsedObj["IsCharging"].as<bool>();
    int batteryPercentage=parsedObj["BatteryPercentage"].as<int>();
    int currentMode=parsedObj["CurrentChargingMode"].as<int>();
    int maxBatteryPercentage=parsedObj["MaxPercentage"].as<int>();
    int minBatteryPercentage=parsedObj["MinPercentage"].as<int>();
      
    JsonDocument responseObj;
    responseObj["overrideActive"]=lastSwitchState;

    if(relayState&&!isCharging&&currentMode!=2){
      responseObj["message"]="Failed to start smart charging";
      responseObj["smartChargingStatus"]=3;
      responseObj["isError"]=true;

      String response;
      serializeJson(responseObj, response);

      server.send(200, "application/json", response);
      return;
    }

    if (lastSwitchState) {
      String response;
      serializeJson(responseObj, response);

      server.send(200, "application/json", response);
      return;
    }

    if (isCharging&&!relayState&&currentMode!=2) {
      responseObj["message"]="Unplug regular charger first";
      responseObj["smartChargingStatus"]=1;
      responseObj["isError"]=true;

      String response;
      serializeJson(responseObj, response);

      server.send(200, "application/json", response);
      return;
    }
 

    String response;
    switch (currentMode) {
      case 0: //best battery life
        responseObj["isError"]=false;
        if (batteryPercentage>=maxBatteryPercentage) {
          waitingForDischarge=true;
        } 
        else if (waitingForDischarge&&batteryPercentage<=minBatteryPercentage) {
        waitingForDischarge=false;     
        }

        if(waitingForDischarge){
          relayState=LOW;

          responseObj["smartChargingStatus"]=2;
          responseObj["message"]="Waiting to discharge to "+String(minBatteryPercentage)+"%";
        }else{
          relayState=HIGH;

          responseObj["smartChargingStatus"]=0;
          responseObj["message"]="Charging to "+String(maxBatteryPercentage)+"%";
        }      
        serializeJson(responseObj, response);

        server.send(200, "application/json", response);
        return;

      case 1: //charge to a given %
        responseObj["isError"]=false;
        if (batteryPercentage<=maxBatteryPercentage) {
          relayState=HIGH;
          responseObj["smartChargingStatus"]=0;
          responseObj["message"]="Charging to "+String(maxBatteryPercentage)+"%";
        } else{
          relayState=LOW;

          responseObj["smartChargingStatus"]=1;
          responseObj["message"]="Finished charging";
        }
        
        serializeJson(responseObj, response);

        server.send(200, "application/json", response);
      return;

      case 2: //off
        relayState=LOW;
        responseObj["isError"]=false;
        responseObj["smartChargingStatus"]=1;
        responseObj["message"]="Not Charging";

        serializeJson(responseObj, response);

        server.send(200, "application/json", response);
      return;
    }
    
    server.send(200);
  });

  int failCounter=0;
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
      Serial.println("Connecting to WiFi...");

    failCounter++;
    if (failCounter>=20) {
    ESP.restart();
    }
  }

  Serial.println("WiFi connected");

  server.begin();

  pinMode(relayPin, OUTPUT);
  digitalWrite(relayPin, relayState);

  pinMode(switchPin, INPUT_PULLUP);
}

void loop() {
  server.handleClient();
  bool currentSwitchState = digitalRead(switchPin);

  if (currentSwitchState != lastSwitchState) {
    // Debounce delay
    delay(50);

    currentSwitchState = digitalRead(switchPin);
    
    if (currentSwitchState != lastSwitchState) {
      // Update the last switch state
      lastSwitchState = currentSwitchState;

      if (lastSwitchState) {
        relayState=HIGH;
      }else{
        relayState=LOW;
      }

      Serial.print("Override switch is now ");
      Serial.println(lastSwitchState);
    }
  }

  if (millis()-currentMillis>=30000 && relayState && !lastSwitchState) {
    Serial.println("Charging disabled since contact to the client was lost.");
    relayState=LOW;
  }

  digitalWrite(relayPin, relayState);
}
