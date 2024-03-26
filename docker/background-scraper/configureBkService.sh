#!/bin/bash

#Creating variables based on received params
for ARGUMENT in "$@"
do
   KEY=$(echo $ARGUMENT | cut -f1 -d=)

   KEY_LENGTH=${#KEY}
   VALUE="${ARGUMENT:$KEY_LENGTH+1}"

   export "$KEY"="$VALUE"
done

validate_args()
{
 [[ -z $DEFAULTCONNECTIONSTRING ]] && exceptionMessage+="DEFAULTCONNECTIONSTRING was not found. "
 [[ -z $FIREBASECREDENTIALFILE ]] && exceptionMessage+="FIREBASECREDENTIALFILE was not found. "
 [[ -z $FIREBASEPROJECTID ]] && exceptionMessage+="FIREBASEPROJECTID was not found. "
 [[ -z $REDISHOST ]] && exceptionMessage+="REDISHOST was not found. "
 [[ -z $REDISPORT ]] && exceptionMessage+="REDISPORT was not found. "
 [[ -z $BROWSERPATH ]] && exceptionMessage+="BROWSERPATH was not found. "
  
 if [ ! -z "$exceptionMessage" ];
 then
   echo "$exceptionMessage"
   exit 2
 fi
}

add_comma()
{
  configurationFile+=","
}

add_connection_scrings()
{
  configurationFile+="\"ConnectionStrings\": {"
  configurationFile+="\"Default\": \"$DEFAULTCONNECTIONSTRING\" "
  configurationFile+="}"
}

add_logging()
{
  configurationFile+="\"Logging\": {"
  configurationFile+="}"
}

add_firebase_settings()
{
  configurationFile+="\"Firebase\": {"
  configurationFile+="\"CredentialFile\": \"$FIREBASECREDENTIALFILE\" "
  add_comma
  configurationFile+="\"ProjectId\": \"$FIREBASEPROJECTID\" "
  configurationFile+="}"
}

add_redis_settings()
{
  configurationFile+="\"Redis\": {"
  configurationFile+="\"Host\": \"$REDISHOST\" "
  add_comma
  configurationFile+="\"Port\": \"$REDISPORT\" "
  configurationFile+="}"
}

add_puppeter_sharp_settings()
{
  configurationFile+="\"PuppeteerSharp\": {"
  configurationFile+="\"ExecutablePath\": \"$BROWSERPATH\" "
  configurationFile+="}"
}

create_configuration_file()
{
  configurationFile="{"
  add_connection_scrings
  add_comma
  add_logging
  add_comma
  add_firebase_settings
  add_comma
  add_redis_settings
  add_comma
  add_puppeter_sharp_settings
  configurationFile+="}"
}

echo "validating args"
validate_args

echo "creating configuration file"
create_configuration_file

#copy config file to container
echo "$configurationFile"
echo "$configurationFile" > appsettings.json

#add permission to config file
chmod +x appsettings.json

#run service
echo "Running scraper background service..."
dotnet Scraper.BackgroundTasks.dll

#how to use:
# docker run 
# -ti 
# -v host_firebase_credentials/application_default_credentials.json:container_firebase_credentials/application_default_credentials.json:ro 
# --network=scrapp-network 
#--rm scraper ./configureBkService.sh 
# DEFAULTCONNECTIONSTRING="scraper database connection string" 
# FIREBASECREDENTIALFILE="/container_firebase_credentials_file/application_default_credentials.json" 
# FIREBASEPROJECTID="firebase_project_id" 
# REDISHOST="redis host. If redis running in container, use container name" 
# REDISPORT="redis port. Default: 6379" 
# BROWSERPATH="default path where chrome is installed inside container, usually: /opt/google/chrome/chrome"
