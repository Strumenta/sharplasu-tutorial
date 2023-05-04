#!/bin/bash
ANTLR_JAR=antlr-4.12.0-complete.jar
java -jar "tools/$ANTLR_JAR" \
  -Dlanguage=CSharp \
  -package "Strumenta.Python3Parser" \
  -o "parser/src/generated" \
  -Xexact-output-dir \
  -no-listener \
  -no-visitor \
  "parser/src/antlr/Python3Lexer.g4" \
  "parser/src/antlr/Python3Parser.g4"
