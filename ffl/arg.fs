\ ==============================================================================
\
\                 arg - the argument parser in the ffl
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
\  $Date: 2007-07-09 17:38:09 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] arg.version [IF]

include ffl/snl.fs
include ffl/str.fs

( arg = Arguments parser )
( The arg parser module implements words for parsing command line arguments. )
( Due to the fact that the ANS standard does not specify words for arguments )
( this module has a environmental dependency.                                )


1 constant arg.version


( Private structure )

struct: arg%opt%   ( - n = Get the required size for the argument option structure )
  snn%
  field: arg>opt>snn         \ this structure is extending the single list node
  cell:  arg>opt>data        \ the optional data for the callback
  cell:  arg>opt>xt          \ the callback xt
  cell:  arg>opt>switch      \ is the option a switch (no parameter) ?
  char:  arg>opt>short       \ the short option or 0
  cell:  arg>opt>long        \ the long option or ""
  cell:  arg>opt>descr       \ the description of option
;struct


: arg-opt-init   ( c-addr:descr u c-addr:long u c:short f:switch w:data xt:callback w:opt - = Initialise the arg>opt structure )
  >r
  r@ arg>opt>snn    snn-init
  r@ arg>opt>xt     !
  r@ arg>opt>data   !
  r@ arg>opt>switch !
  r@ arg>opt>short  c!
  str-new dup 
  r@ arg>opt>long   ! str-set
  str-new dup 
  r> arg>opt>descr  ! str-set
;


: arg-opt-new   ( c-addr:descr u c-addr:long u c:short f:switch xt - w:opt = Create a new arg-opt structure on the heap )
  arg%opt% allocate  throw   >r r@ arg-opt-init r>
;


: arg-opt-free   ( w:opt - = Free the arg.opt structure from the heap )
  dup arg>opt>long  str-free
  dup arg>opt>descr str-free
    
  free throw
;


( Argument parser structure )

struct: arg%   ( - n = Get the required space for the arg data structure )
  snl%
  field: arg>options    \ the option list
  cell:  arg>name       \ the program name
  cell:  arg>usage      \ the program usage
  cell:  arg>version    \ the program version
  cell:  arg>tail       \ the program tail
  cell:  arg>data       \ the optional data
  cell:  arg>xt         \ the non-option callback xt
;struct


( Argument parser structure creation, initialisation and destruction )

: arg-init   ( c-addr:name u c-addr:usage u c-addr:version u c-addr:tail u w:arg - = Initialise the argument parser structure )
  >r
  r@ arg>options snl-init
  str-new dup
  r@ arg>tail    ! str-set
  str-new dup
  r@ arg>version ! str-set
  str-new dup
  r@ arg>usage   ! str-set
  str-new dup
  r@ arg>name    ! str-set
  r@ arg>data nil!
  r> arg>xt   nil!
;


: arg-create   ( C: "name"  c-addr:name u c-addr:usage u c-addr:version u c-addr:tail u -  R: - w:arg = Create a named argument parser structure in the dictionary )
  create  here arg% allot  arg-init
;


: arg-new   (  c-addr:name u  c-addr:usage u c-addr:version u c-addr:tail u - w:arg = Create a new argument parser structure on the heap )
  arg% allocate  throw   >r r@ arg-init r>
;


: arg-free   ( w:arg - = Free the argument parser structure from the heap )
  dup arg>name    @ str-free
  dup arg>usage   @ str-free
  dup arg>version @ str-free
  dup arg>tail    @ str-free
  
  dup arg>options                 \ Free all argument options
  BEGIN
    dup snl-remove-first dup nil<>
  WHILE
    arg-opt-free
  REPEAT
  2drop  

  free throw
;



( Private print words )

: arg-print-help   ( w:arg - = Print the help info )
  \ ToDo
;


: arg-print-usage   ( w:art - = Print the usage info )
  \ ToDo
;


( Argument parser words )

: arg-add-option   ( c-addr:descr u c-addr:long u c:short f:switch w:data xt:callback w:arg - = Add an option to the argument parser )
  \ ToDo
;


: arg-parse   ( w:data xt:callback w:arg - f  = Parse the command line arguments )
  \ ToDo
;


( Inspection )

: arg-dump   ( w:arg - = Dump the arg state )
  \ ToDo
;

[THEN]

\ ==============================================================================
