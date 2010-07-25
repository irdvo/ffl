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
\  $Date: 2007-12-09 07:23:14 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/arg.fs

\ Test if the argument parser is implemented for the current forth Engine

[DEFINED] arg.version [IF]

\ Create an argument parser on the heap

s" argparser"                       \ program name
s" [OPTION] .. [FILES]"             \ program usage
s" v1.0"                            \ program version
s" Report bugs to bugs@bugs.com"    \ program extra info
arg-new value arg1


\ Add the default help and version options

arg1 arg-add-help-option

arg1 arg-add-version-option


\ Variable for the verbose switch

variable verbose  verbose off


\ Add the -v/--verbose option switch

char v                              \ Short option name
s" verbose"                         \ Long option name
s" activate verbose mode"           \ Description
true                                \ Switch -> true
4                                   \ Option id
arg1 arg-add-option

     
\ Add the -f/--file=FILE option

char f                              \ Short option name
s" file=FILE"                       \ Long option name
s" set input file, any input file is allowed, as long as the description is multicolumn"  \ Description
false                               \ Parameter -> false
5                                   \ Option id
arg1 arg-add-option


: parse-options ( -- )
  BEGIN
    arg1 arg-parse                 \ parse the next argument
    dup arg.done <> over arg.error <> AND  \ stop parsing when ready or after an error
  WHILE
    
    CASE
      arg.help-option    OF arg1 arg-print-help             ENDOF  \ print default help info
    
      arg.version-option OF arg1 arg-print-version          ENDOF  \ print default version info
      
      arg.non-option     OF ." Non option found:" type cr   ENDOF  \ non option parameter, parameter on stack
      
      4                  OF verbose on ." Verbose is on" cr ENDOF  \ switch, no extra stack parameters
      
      5                  OF ." File parameter:" type cr     ENDOF  \ parameter switch, parameter on stack
    ENDCASE
  REPEAT
  
  arg.done = IF
    ." All options okee." cr
  ELSE
    ." Error in one of the options." cr
  THEN
;  

\ Parse the command line arguments

parse-options

\ Free the argument parser from the heap

arg1 arg-free

[THEN]

