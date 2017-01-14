#!/bin/bash

PACK_COMMAND="docker-compose run --rm praxis-cli dotnet pack --include-source --configuration Release"
PUSH_COMMAND_TEMPLATE="docker-compose run --rm praxis-cli mono /usr/local/bin/nuget push ./bin/Debug/Praxis*.nupkg NUGET_API_KEY -Source nuget.org"

if [ "$TRAVIS_BRANCH" == "master" ]; then

  if [ -z "$NUGET_API_KEY" ]; then
    echo "Missing nuget API key, unable to release."
  else
    echo "This is a new release, yay!  Publishing nuget package."
	PUSH_COMMAND="${PUSH_COMMAND_TEMPLATE/NUGET_API_KEY/$NUGET_API_KEY}"
	$PACK_COMMAND
    $PUSH_COMMAND
  fi

else
  echo "Not a release, skipping nuget publish."
  echo "Commands run would be:"
  echo $PACK_COMMAND
  echo $PUSH_COMMAND_TEMPLATE
fi
