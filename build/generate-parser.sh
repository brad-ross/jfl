#!/bin/bash

if [ `which mono` ]; then
	mono ./build/Antlr/Antlr3.exe -fo ./src/Parser ./src/Parser/Grammar/JFL.g
	mono ./build/Antlr/Antlr3.exe -fo ./src/Parser ./src/Parser/Grammar/JFLWalker.g
	
	mv ./src/Parser/JFL.tokens ./src/Parser/JFLWalker.tokens ./src/Parser/Grammar
else
	echo Mono must be installed to build.
fi