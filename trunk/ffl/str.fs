\ ==============================================================================
\
\             str - the character string module in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2005-12-24 08:38:48 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] str.version [IF]


include ffl/stc.fs
include ffl/chr.fs


( str = Character string )
( The str module implements a character string.)


1 constant str.version


( Public structure )

struct: str%       ( - n = Get the required space for the str data structure )
  cell: str>data
  cell: str>length
  cell: str>size    
  cell: str>extra
;struct


( Private database )
8 value str.extra   ( - w = the initial extra space )


( Private words )



( Public words )

: str-init         ( w:str - = Initialise the empty string )
  dup str>data  nil!
  dup str>length  0!
  dup str>size    0!
  str.extra swap str>extra !
;


: str-create       ( C: "name" - R: - w:str = Create a named string in the dictionary )
  create   here   str% allot   str-init
;


: str-new          ( - w:str = Create a new string on the heap )
  str% allocate  throw  dup str-init
;


: str-free         ( w:str - = Free the string from the heap )
  free  throw
;


: str-empty?       ( w:str - f = Check for empty string )
  str>length @ 0=  
;


: str-length@      ( w:str - u = Get the length of the string )
  str>length @
;


: str-size!        ( u w:str - = Insure the size of the string )
  dup str>data @ nil= IF     \ if data = nil then
    tuck str>extra @ +       \   size = requested + extra
    2dup swap str>size !
    1+ chars allocate throw  \ 
    swap str>data !          \   data = allocated size + 1 for zero terminated string
  ELSE
    2dup str>size @ > IF     \ else if requested > current size then
      tuck str>extra @ +     \   size = requested + extra
      2dup swap str>size !
      1+ chars
      over str>data @ swap   \   reserve extra character for zero terminated string
      resize throw        
      swap str>data !    
    ELSE
      2drop
    THEN
  THEN
;


: str-extra@       ( w:str - u = Get the extra space allocated during resizing of the string )
  str>extra @
;

: str-extra!       ( u w:str - = Set the extra space allocated during resizing of the string )
  str>extra !
;


: str+extra@       ( - u = Get the initial extra space allocated during resizing of the string )
  str.extra
;


: str+extra!       ( u - = Set the initial extra space allocated during resizing of the string )
  to str.extra
;


( String manipulation )


: str-clear        ( w:str - = Clear the string )
  str>length 0!
;


: str-set          ( c-addr u w:str - = Set a string in the string )
;


: str-set"         ( ") ( w:str - = Set a string in the string )
;


: str-append       ( c-addr u w:str - = Append a string to the string )
;


: str-append"      ( ") ( w:str - = Append a string to the string )
;

: str-prepend      ( w:str - = Prepend a string to the string )
;


: str-prepend"     ( ") ( w:str - = Prepend a string to the string )
;


: str-insert       ( c-addr u w:start w:str - = Insert a string in the string )
;


: str-insert"      ( ") ( w:start w:str - = Insert a string in the string )
;


: str-substring    ( w:start w:end w:str - w:str = Get a substring as a new string )
;


: str-get          ( w:str - c-addr u = Get the string )
;


: str-delete       ( w:start w:end w:str - = Delete a range from the string )
;


: str-set-cstring  ( c-addr w:str - = Set a zero terminated string in the string )
;


: str-get-cstring  ( w:str - c-addr = Get the string as zero terminated string )
;


: str^move         ( w:str2 w:str1 - Move str2 in str1 )
;


( Character words )

: str-push         ( c w:str - = Push a character at the end of the string )
;


: str-pop          ( w:str - c = Pop a character from the end of the string )
;


: str-enqueue      ( c w:str - = Place a character at the start of the string )
;


: str-dequeue      ( c w:str - = Get the character at the end of the string )
;


: str-set-char     ( c n w:str - = Set the character on the nth position in the string )
;


: str-get-char     ( n w:str - c = Get the character from the nth position in the string )
;


: str-insert-char  ( c n w:str - = Insert the character on the nth position in the string )
;


: str-delete-char  ( c n w:str - = Delete the character on the nth position in the string )
;


: str-execute      ( ... xt w:str - ... = Execute the xt token for every character in the string )
;


( Special changes )

: str-capatilize   ( w:str - = Capatilize the first word in the string )
;


: str-cap-words    ( w:str - = Capatilize all words in the string )
;


: str-center       ( u w:str - = Center the string in u width )
;


: str-ljust        ( u w:str - = Left justify the string )
;


: str-rjust        ( u w:str - = Right justify the string )
;


: str-zfill        ( u w:str - = Right justify the string with leading zero's )
;


: str-strip        ( w:str - = Strip leading and trailing spaces in the string )
;


: str-lstrip       ( w:str - = Strip leading spaces in the string )
;


: str-rstrip       ( w:str - = Strip trailing spaces in the string )
;


: str-lower        ( w:str - = Convert the string to lower case )
;


: str-upper        ( w:str - = Convert the string to upper case )
;


: str-expand-tabs  ( u w:str - = Expand the tabs in the string )
;


( Comparison )


: str^icompare     ( w:str w:str - n = Compare case-insensitive two strings )
;


: str^ccompare     ( w:str w:str - n = Compare case-sensitive two strings )
;


: str-icompare     ( c-addr u w:str - n = Compare case-insensitive a string with the string )
;


: str-icompare"    ( ") ( w:str - n = compare case-insensitive a string with the string )
;


: str-ccompare     ( c-addr u w:str - n = Compare case-sensitive a string with the string )
;


: str-ccompare"    ( ") ( w:str - n = compare case-sensitive a string the string )
;


: str-count        ( c-addr u w:str - u = Count the number of occurences of a string in the string )
;


: str-find         ( c-addr u n w:str - n = Find the first occurence of a string from nth position in the string )
;


: str-replace      ( c-addr u c-addr u w:str - n = Replace the occurences of the first string with the second string in the string )
;


[THEN]

\ ==============================================================================
