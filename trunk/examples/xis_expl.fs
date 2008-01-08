\ ==============================================================================
\
\          tis_expl - the text input stream example in the ffl
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
\  $Date: 2008-01-08 19:20:16 $ $Revision: 1.2 $
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



s" index.xml" r/o open-file throw value xis.file  \ Open the file

xis.file  ' file-reader   xis1 xis-set-reader     \ Use the xml reader with a file

true xis1 xis-strip!                              \ Strip leading and trailing spacewhite in the text


: ?type ( c-addr u - = Print the string with zero length check )
  dup IF
    type
  ELSE
    2drop
    ." <empty>"
  THEN
;


: print-attributes ( c-addrn un c-addr un .. n  -- Print all attributes )
  0 ?DO                                 \ Do for all attributes
    ."  Attribute: " type               \   Print attribute name
    ."  Value: " ?type                  \   Print attribute value
  LOOP
;


: file-parse  ( -- = Parse the xml file )
  BEGIN
    xis1 xis-read                           \ Read the next token from the file
    dup xis.error <> over xis.done <> AND   \ Done when ready or error
  WHILE
    CASE                                    \ Depending on the parsed token: print the parameters
      xis.start-xml     OF ." Start XML document:" print-attributes cr                                              ENDOF
      xis.comment       OF ." Comment: " type cr                                                                    ENDOF
      xis.text          OF ." Text: " type cr                                                                       ENDOF
      xis.start-tag     OF ." Start tag: " type print-attributes cr                                                 ENDOF
      xis.end-tag       OF ." End tag: " type cr                                                                    ENDOF
      xis.empty-element OF ." Empty element: " type cr print-attributes cr                                          ENDOF
      xis.cdata         OF ." CDATA section: " type cr                                                              ENDOF
      xis.proc-instr    OF ." Proc. Instr.: " type cr print-attributes cr                                           ENDOF
      xis.internal-dtd  OF ." Internal DTD: " type ."  Markup: " type cr                                            ENDOF
      xis.public-dtd    OF ." Public DTD: " type ."  Markup: " ?type ."  SystemID: " ?type ."  PublicID: " ?type cr ENDOF
      xis.system-dtd    OF ." System DTD: " type ."  Markup: " ?type ."  SystemID: " ?type cr                       ENDOF
    ENDCASE
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

