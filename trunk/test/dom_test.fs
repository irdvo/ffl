\ ==============================================================================
\
\        xos_test - the test words for the xos module in the ffl
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
\  $Date: 2008-02-18 06:35:28 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/dom.fs
include ffl/tst.fs


.( Testing: dom) cr

t{ dom-create dom1 }t


: dom-test-reader   ( file-id -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap 
    \ 2dup type cr
  THEN
;

t{ s" test.xml" r/o open-file throw value dom.file }t

t{ dom.file ' dom-test-reader dom1 dom-read-reader ?true }t

\ Iterate and modify the tree

t{ dom1 dom-document ?true }t

t{ dom1 dom-document? ?true }t

t{ dom1 dom-parent ?false }t

t{ dom1 dom-document ?true }t

t{ dom1 dom-children 7 ?s }t

t{ dom1 dom-children? ?true }t

t{ dom1 dom-child  ?true dom.attribute ?s }t

t{ dom1 dom-first?    ?true  }t
t{ dom1 dom-last?     ?false }t
t{ dom1 dom-children? ?false }t

t{ dom1 dom-get-name  s" standalone" ?str }t
t{ dom1 dom-get-value s" no"     ?str }t


t{ dom1 dom-next ?true dom.attribute ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?false }t
t{ dom1 dom-children? ?false }t

t{ dom1 dom-get-name  s" version" ?str }t
t{ dom1 dom-get-value s" 1.0"     ?str }t

t{ s" version" s" 1.1" dom1 dom-set }t

t{ dom1 dom-next ?true dom.text ?s }t

t{ dom1 dom-next ?true dom.comment ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?false }t
t{ dom1 dom-children? ?false }t

t{ dom1 dom-get-value s"  This is a test file for the ffl-library " ?str }t

t{ s"  This is a (modified) test file " dom1 dom-set }t

t{ dom1 dom-next ?true dom.text ?s }t

t{ dom1 dom-next ?true dom.element ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?false }t
t{ dom1 dom-children? ?true  }t

t{ dom1 dom-next ?true dom.text ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?true  }t
t{ dom1 dom-children? ?false }t

t{ dom1 dom-prev ?true dom.element ?s }t

t{ dom1 dom-get-name s" TEST" ?str }t

t{ dom1 dom-child ?true dom.text ?s }t

t{ dom1 dom-next ?true dom.element ?s }t

t{ dom1 dom-get-name s" BOOK" ?str }t

t{ s" BOOKLET" dom1 dom-set }t

t{ s" SUBJECT" dom.element dom1 dom-append-node }t

t{ s" MAIN" s" Computers"   dom.attribute dom1 dom-append-node }t
t{ s" SUB"  s" Programming" dom.attribute dom1 dom-insert-node-before }t
t{ s" Introduction to the " dom.text dom1 dom-insert-node-after }t
t{ dom1 dom-next ?true dom.text ?s }t
t{ s" forth programming language" dom.text dom1 dom-insert-node-after }t

t{ dom1 dom-parent ?true dom.element ?s }t
t{ dom1 dom-parent ?true dom.element ?s }t  \ back to booklet

t{ dom1 dom-child ?true dom.text ?s }t
t{ dom1 dom-next  ?true dom.element ?s }t
t{ dom1 dom-next  ?true dom.text ?s }t
t{ dom1 dom-next  ?true dom.element ?s }t

t{ dom1 dom-child ?true dom.text ?s }t
t{ dom1 dom-next  ?true dom.element ?s }t
t{ dom1 dom-next  ?true dom.text ?s }t
t{ dom1 dom-next  ?true dom.element ?s }t

t{ dom1 dom-get-name s" FIRSTBOOK" ?str }t

t{ dom1 dom-remove ?true }t  \ remove first book
t{ dom1 dom-last?  ?true }t
t{ dom1 dom-remove ?true }t  \ remove trailing spaces

\ Write the tree

t{ dom1 dom-write-string ?true type }t

\ ==============================================================================
