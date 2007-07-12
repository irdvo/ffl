\ ==============================================================================
\
\          arg_expl - the argument parser example in the ffl
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
\  $Date: 2007-07-12 18:29:53 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/arg.fs

\ Variables for storing parsing results
variable verbose  verbose off

\ Create an argument parser on the heap

s" argparser"                    \ program name
s" [OPTION] .. [FILES]"          \ program usage
s" v1.0"                         \ program version
s" Report bugs to bugs@bugs.com" \ program tail
arg-new value arg1

\ Add the default help and version options

arg1 arg-add-default-options


\ Create the verbose! callback function

: verbose!  ( addr - f )
  on                          \ set the verbose variable to on
  true                        \ return true: continue
;

\ Add the verbose option: short name=v long name=verbose 
\ description=activate verbose mode switch=true data=verbose xt=verbose!
  
char v  s" verbose" s" activate verbose mode" true verbose ' verbose! arg1 arg-add-option


\ Create the default callback function

: non-option ( c-addr u data - f )
  drop 
  ." Non option found: " type cr
  true
;

nil ' non-option arg1 arg-parse drop

\ Free the argument parser from the heap

\ arg1 arg-free

