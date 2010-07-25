\ ==============================================================================
\
\          xis_expl - the text input stream example in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2008-11-16 18:55:14 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/xis.fs


\ Example: Read a XML/HTML file

\ Create a XML/HTML input stream on the heap

xis-new value xis1


\ Setup the reader callback word for reading from file

: file-reader ( fileid -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap
  THEN
;



s" test.xml" r/o open-file throw value xis.file  \ Open the file

xis.file  ' file-reader   xis1 xis-set-reader     \ Use the xml reader with a file

true xis1 xis-strip!                              \ Strip leading and trailing whitespace in the text

: file-parse  ( -- = Parse the xml file )
  BEGIN
    xis1 xis-read                           \ Read the next token from the file
    dup xis.error <> over xis.done <> AND   \ Done when ready or error
  WHILE
    xis+dump-read-parameters                \ Dump the next token with its parameters
   
  REPEAT
  
  xis.error = IF
    ." Error parsing the file." cr
  ELSE
    ." File succesfully parsed." cr
  THEN
;

\ Parse the file

file-parse


\ Done, close the file

xis.file close-file throw


\ Free the stream from the heap

xis1 xis-free

