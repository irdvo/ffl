\ ==============================================================================
\
\            enm_expl - the enumeration example in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-02-24 07:43:43 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/enm.fs


\ Example: number names

begin-enumeration
  enum: zero            \ Default: an enum start with 0
  enum: one
  enum: two
  enum: three
  enum: four
  enum: five
end-enumeration

.( Enum zero: ) zero . .(  and three: ) three . cr


\ Example: month names

begin-enumeration
  1
  >enum: january        \ Start this enumeration with 1
  enum:  february
  enum:  march
  enum:  april
  enum:  may
  enum:  june
  enum:  july
  enum:  august
  enum:  september
  enum:  october
  enum:  november
  enum:  december
end-enumeration

.( January: ) january . .( and december: ) december . cr
