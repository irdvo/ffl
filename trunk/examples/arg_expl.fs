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
\  $Date: 2007-07-18 19:16:09 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/arg.fs


\ Create an argument parser on the heap

s" argparser"                       \ program name
s" [OPTION] .. [FILES]"             \ program usage
s" v1.0"                            \ program version
s" Report bugs to bugs@bugs.com"    \ program extra info
arg-new value arg1


\ Add the default help and version options

arg1 arg-add-default-options


\ Create the callback word for the -v/--verbose switch option

variable verbose  verbose off

: verbose!  ( addr - f )
  on                                \ in this example: data contains the variable to set on
  ." Verbose option found.." cr     \ give some info
  true                              \ return true: continue
;

\ Add the -v/--verbose option switch
  
char v                              \ Short option name
s" verbose"                         \ Long option name
s" activate verbose mode"           \ Description
true                                \ Switch -> true
verbose                             \ Destination data
' verbose!                          \ Callback word
arg1 arg-add-option

     
\ Create the callback word for the -f/--file=FILE parameter option

: print-parameter ( c-addr u nil - f )
  drop                              \ in this example: no interest in the data, ..
  ?dup IF                           \ Check if parameter is not empty
    ." File parameter: " type cr    \ .. type the parameter, ..
    true                            \ .. and continu
  ELSE                              \ Else
    drop
    ." Error: empty parameter" type cr
    false                           \   Error and stop parsing
  THEN
;

\ Add the -f/--file=FILE option

char f                              \ Short option name
s" file=FILE"                       \ Long option name
s" set input file, any input file is allowed, as long as the description is multicolumn"  \ Description
false                               \ Parameter -> false
nil                                 \ No destination data
' print-parameter                   \ Callback word
arg1 arg-add-option


\ Create the non-option callback word

: non-option ( c-addr u data - f )
  drop                             \ in this example: no interest in the data, ..
  ." Non option found: " type cr   \ .. some feedback, ..
  true                             \ .. and continu
;


\ Parse the command line arguments

nil ' non-option arg1 arg-parse [IF]
  .( All options okee !) cr
[ELSE]
  .( Error in one of the options, or help or version info requested.) cr
[THEN]


\ Free the argument parser from the heap

arg1 arg-free

