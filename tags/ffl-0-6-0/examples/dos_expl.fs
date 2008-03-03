\ ==============================================================================
\
\          dos_expl - the date output stream example in the ffl
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
\  $Date: 2007-11-22 19:15:25 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/dos.fs
include ffl/gmo.fs



\ Example 1: Format the output with words

\ Create the date and output stream in the dictionary

dtm-create dtm1
tos-create tos1

\ Set the date with 22 november 2007 15:00:59

0 59 00 15 22 dtm.may 2007 dtm1 dtm-set

\ Format the output stream

dtm1 tos1 dos-write-date-time

tos1 str-get type cr                 \ Shows: 2007/05/22 15:00:59

tos1 tos-rewrite                     \ Clean the stream

dtm1 tos1 dos-write-weekday-name

tos1 str-get type cr                 \ Shows: Tuesday



\ Example 2: Format the output with a string format

tos1 tos-rewrite

dtm1 s" %a %b %e %H:%M:%S %Y" tos1 dos-write-format

tos1 str-get type cr                 \ Shows: Tue May 22 15:00:59 2007



\ Example 3: Use a message catalog for localisation of dates

msc-create en>nl                     \ Create the message catalog

s" nl.mo" en>nl gmo-read throw       \ Import the nl.mo file in the catalog

en>nl tos1 tos-msc!                  \ Let the ouput stream use the message catalog


tos1 tos-rewrite                     \ Clear the output stream

dtm1 s" %a %b %e %H:%M:%S %Y" tos1 dos-write-format

tos1 str-get type cr                 \ Shows: din mei 22 15:00:59 2007 (dutch)
