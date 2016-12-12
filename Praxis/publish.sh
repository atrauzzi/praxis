#!/bin/bash

if [ "$TRAVIS_BRANCH" == "master" ] && [ ! -z "$TRAVIS_TAG" ]; then

  if [ -z "$NUGET_API_KEY" ]; then
    echo "Missing nuget API key, unable to release."
  else
    echo "This is a new release, yay!  Publishing nuget package."
    docker-compose run --rm praxis-cli mono /usr/local/bin/nuget push ./bin/Debug/Praxis.$TRAVIS_TAG.nupkg $NUGET_API_KEY -Source nuget.org
  fi

else
  echo "Not a release, skipping nuget publish."
fi
