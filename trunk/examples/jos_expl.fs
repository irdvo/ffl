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

include ffl/jos.fs


\ Example : Write json output


\ Create the json output stream in the dictionary

jos-create jos1

jos1 jos-write-start-object           \ Write the start of a json object 

s" number" jos1 jos-write-name        \ Write a number with its value
5          jos1 jos-write-number

s" array"  jos1 jos-write-name        \ Write an array with three members
           jos1 jos-write-start-array
7          jos1 jos-write-number
true       jos1 jos-write-boolean
s" hello"  jos1 jos-write-string
           jos1 jos-write-end-array

jos1 jos-write-end-object             \ Write the end of the object

jos1 str-get type cr                  \ Type the json output
