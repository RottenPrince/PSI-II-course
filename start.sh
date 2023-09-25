#!/bin/sh

set -e

SOLUTION_PATH="./Solution/"
SOLUTION_FILE="Solution.sln"

SOLUTION_FILE_FULL="$SOLUTION_PATH/$SOLUTION_FILE"
PROJECTS=$(dotnet sln $SOLUTION_FILE_FULL list | tail -n +3)

dotnet build $SOLUTION_FILE_FULL

trap 'echo "CTRL-C caught, exiting..."; kill 0' SIGINT
for PROJECT in $PROJECTS
do
	PROJECT_PATH_FULL="$SOLUTION_PATH/$PROJECT"
	dotnet run --no-build --project $PROJECT_PATH_FULL &
done
wait
echo "Done..."
