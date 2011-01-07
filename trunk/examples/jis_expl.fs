\ ==============================================================================
\
\          jos_expl - the json output stream example in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/jis.fs


\ Example : Parse a json string


\ Create the json input stream in the dictionary

jis-create jis1

\ Feed the parser with a string

s\" {\"value\":10,\"flag\":false,\"array\":[1,2,3.1],\"string\":\"hello\",\"empty\":\"\"}" jis1 jis-set-string

\ Parse the string and print the parsed tokens

: json-parse  ( -- = Parse the string )
  BEGIN
    jis1 jis-read
    dup  jis.error <> over jis.done <> AND
  WHILE
    CASE
      jis.start-object OF ." Start Object"     ENDOF
      jis.start-array  OF ." Start array"      ENDOF
      jis.name         OF ." Name : "     type ENDOF
      jis.string       OF ." String : "   type ENDOF
      jis.number       OF ." Number : "   .    ENDOF 
      jis.double       OF ." Double : "   d.   ENDOF
      jis.float        OF ." Float : "    f.   ENDOF
      jis.boolean      OF ." Boolean : "  .    ENDOF
      jis.null         OF ." Null"             ENDOF
      jis.end-object   OF ." End Object"       ENDOF
      jis.end-array    OF ." End Array"        ENDOF
      ." Token:" dup .
    ENDCASE
    cr
  REPEAT
  drop
;

json-parse
