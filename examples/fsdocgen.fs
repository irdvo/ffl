\ ==============================================================================
\
\         fsdocgen - ffl documentation generator
\
\               Copyright (C) 2015  Dick van Oudheusden
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
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/stc.fs
include ffl/str.fs
include ffl/car.fs


[DEFINED] #args [DEFINED] arg@ AND [IF]

\ ==============================================================================
\ fswd module -- the forth source document generator word
\ ==============================================================================

\ ------------------------------------------------------------------------------
\ Data structure
\ ------------------------------------------------------------------------------

begin-structure fswd%
  str%
  +field   fswd>word
  str%
  +field   fswd>stack
end-structure

\ ------------------------------------------------------------------------------
\ allocator
\ ------------------------------------------------------------------------------

: fswd-new ( str1  str2 -- fswd = Allocate a new generator 1:word with 2:stack)
  fswd% allocate throw
  >r
  r@ fswd>stack str-init
  str-get
  r@ fswd>stack str-set
  
  r@ fswd>word  str-init
  str-get
  r@ fswd>word  str-set
  r>
;

\ ------------------------------------------------------------------------------
\ Deallocator
\ ------------------------------------------------------------------------------

: fswd-free  ( fswd -- = Free the generator word )
  >r
  r@ fswd>word  str-(free)
  f@ fswd>stack str-(free)
  r>
  free throw
;

\ ------------------------------------------------------------------------------
\ Compare
\ ------------------------------------------------------------------------------

: fswd^ccompare  ( fswd1 fswd2 -- n = Compare two generator words )
  swap fswd>word swap fswd>word str^ccompare
;

\ ------------------------------------------------------------------------------
\ Get
\ ------------------------------------------------------------------------------

: fswd-get  ( fswd -- str1 str2 = Get the generator 1:word and 2:stack )
  dup fswd>word swap fswd>stack
;

\ ==============================================================================
\ fsdg module -- the forth source document generator
\ ==============================================================================

\ ------------------------------------------------------------------------------
\ Data structure
\ ------------------------------------------------------------------------------

begin-structure fsdg%
  str%
  +field   fsdg>output-directory
  str%
  +field   fsdg>example-directory
  car%
  +field   fsdg>input-files
  car%
  +field   fsdg>index
end-structure


\ ------------------------------------------------------------------------------
\ Allocators
\ ------------------------------------------------------------------------------

: fsdg-new  ( -- fsdg = Allocate a fsdg variable )
  fsdg% allocate throw
  >r
  r@ fsdg>output-directory  str-init
  r@ fsdg>example-directory str-init
  0 
  r@ fsdg>input-files       car-init
  256
  r@ fsdg>input-files       car-size!
  0
  r@ fsdg>index             car-init
  1024
  r@ fsdg>index             car-size!
  512
  r@ fsdg>index             car-extra!           \ Let the array increase faster
  ['] fswd^ccompare
  r@ fsdg>index             car-compare!
  r>
;

\ ------------------------------------------------------------------------------
\ Deallocators
\ ------------------------------------------------------------------------------

: fsdg-free  ( fsdg -- = Free the fsdg variable )
  >r
  r@ fsdg>output-directory  str-(free)
  r@ fsdg>example-directory str-(free)
  ['] str-free  r@ fsdg>input-files car-execute
  r@ fsdg>input-files       car-(free)
  ['] fswd-free r@ fsdg>index       car-execute
  r@ fsdg>index             car-(free)
  r> free throw
;

\ ------------------------------------------------------------------------------
\ Read the program arguments
\ ------------------------------------------------------------------------------

: fsdg-store-file-arguments ( car -- = Store the file arguments in the array )
  #args 2 DO
    i arg@                                       \ Read the argument, ..
    str-new >r r@ str-set r>                     \ .. store it in dyn. string ..
    over car-append                              \ .. and append in array
  LOOP
  drop
;

: fsdg-read-arguments ( fsdg -- = Read the command line arguments )
  >r
  #args 3 < IF  
    ." usage: fsdocgen  OUTPUT DIRECTORY  EXAMPLE DIRECTORY  FILE .." cr
    ." The html documentation generator for the ffl-library." cr cr
  ELSE
    0 arg@ r@ fsdg>output-directory  str-set
    1 arg@ r@ fsdg>example-directory str-set

    r@ fsdg>input-files fsdg-store-file-arguments    
  THEN
  rdrop
;

\ ------------------------------------------------------------------------------
\ Convert the source modules to html files
\ ------------------------------------------------------------------------------

: fsdg-write-module  ( fsdg str -- fsdg = Convert a source module to html )
  \ XXX: Create output file -> fileid
  \ XXX: Open the input file -> fileid
  \ XXX:   Read and scan a line fileid -> fswd
  \ XXX:   Store filename in fswd str fswd ->
  \ XXX:   Generate html   fileid fswd ->
  \ XXX:   Store fswd in fsdg>index
  \ XXX: Close input file
  \ XXX: Close output file
  drop
;

: fsdg-write-modules  ( fsdg -- = Convert the source modules to html )
  ['] fsdg-write-module  over fsdg>input-files  car-execute
  drop
;

\ ------------------------------------------------------------------------------
\ Write the word index file
\ ------------------------------------------------------------------------------

: fsdg-write-index  ( fsdg -- = Write the word index file )
  \ XXX
  drop
;

\ ==============================================================================
\ Main program
\ ==============================================================================

: fsdocgen  ( -- = Main program )
  fsdg-new >r
  r@ fsdg-read-arguments
  r@ fsdg-write-modules
  r@ fsdg-write-index
  r> fsdg-free
;

fsdocgen

.s \ XXX 

[ELSE]
  .( fsdocgen: the forth engine does not support command line arguments. )
[THEN]

bye

