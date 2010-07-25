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
\  $Date: 2008-03-11 18:33:47 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/tis.fs


\ Example 1: Use the text input stream with a string of text


\ Create an text input stream in the dictionary

tis-create tis1


\ Fill the stream with a string

s" This is a special test string" tis1 tis-set


\ Match the start of the string

char t tis1 tis-imatch-char [IF]            \ Match the start of the string with a t (case insensitive)
  .( The string starts with a t or T.) cr
[THEN]
  
s" his i" tis1 tis-cmatch-string [IF]
  .( After that the string starts with his.)  cr \ After matching the t, the string 'his i' is matched
[THEN]


\ Read in the string

tis1 tis-read-char [IF]
  .( Next character: ) emit cr                \ The next character is read (e.g. s)
[THEN]

.( Next five characters: )
5 tis1 tis-read-string type cr                \ The next five characters are read (e.g. ' a sp')


\ Scan for string

s" test" tis1 tis-scan-string [IF]
  .( String till 'test': ) type cr            \ Return the skipped text (e.g. 'ecial ')
[THEN]


\ Skip spaces

.( Skipped spaces: )
tis1 tis-skip-spaces . cr                     \ Skip spaces and return the number of skipped spaces (e.g. 1)
  


\ Example 2: Use the text input stream with a reader

\ Create a text input stream on the heap

tis-new value tis2


\ Setup the reader callback word

: tis-reader ( fileid -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap
  THEN
;

s" index.html" r/o open-file throw dup  ' tis-reader   tis2 tis-set-reader   \ Setup a reader with a file


\ Scan with the reader 
: show-links   ( -- = example: Type all links in html file )
  ." Links:" cr
  BEGIN
    true
    s" <a HREF=" tis2 tis-iscan-string IF        \ Look for '<a HREF=', case insensitive, save lookup result
      2drop                                      \ No interest in leading string
      0=
      [char] > tis2 tis-scan-char IF             \ Look for '>'
        type cr                                  \ Type leading string = link
      THEN
    THEN
  UNTIL
;

show-links

\ Done, close the file

close-file throw

\ Free the stream from the heap

tis2 tis-free

