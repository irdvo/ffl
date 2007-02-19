\ ==============================================================================
\
\          snn_expl - the single list node example in the ffl
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
\  $Date: 2007-02-19 18:52:45 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/snn.fs

 \
 \ the single list node is base structure for storing data in a single linked list
 \
 \ For example: a book collection in a single linked list:
 \  a book is defined by a title (20 chars), an author (20 chars) and a number
 \
 
struct: book%
  snn% field: book>snn       \ book is part of a single list and
    20 chars: book>title     \ a title and
    20 chars: book>author    \ an author and
       cell:  book>number    \ a number
;struct

 \
 \ Next create a number of books
 \
 
: book-new ( c-addr u c-addr u n - w:book )
  book% allocate throw >r                            \ allocate a new book
  r@ book>snn snn-init                               \ initialise for the list
  r@ book>number !                                   \ store the book number
  \ store author
  \ store title
  r>
;

s" Lord of the rings" s" Tolkien" 1 book-new
 
