\ ==============================================================================
\
\         gmo_expl - the gettexts mo-file import example in the ffl
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
\  $Date: 2007-11-17 07:47:22 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/gmo.fs


\ Example: import nl.mo file in a message catalog

msc-new value en>nl                 \ Create a message catalog on the heap

s" nl.mo" en>nl gmo-read throw      \ Import the nl.mo file in the catalog

s" Sunday" en>nl msc-translate type cr  \ Use the catalog for the translations

en>nl msc-free                      \ Free the message catalog from the heap
