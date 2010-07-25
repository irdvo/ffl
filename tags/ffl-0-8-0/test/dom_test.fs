\ ==============================================================================
\
\        dom_test - the test words for the dom module in the ffl
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
\  $Date: 2008-04-15 17:13:54 $ $Revision: 1.10 $
\
\ ==============================================================================

include ffl/dom.fs
include ffl/tst.fs
include ffl/est.fs


.( Testing: dom) cr

t{ dom-new value dom1 }t


: dom-test-reader   ( file-id -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap 
    \ 2dup type cr
  THEN
;

t{ s" test.xml" r/o open-file throw value dom.file }t

t{ dom.file ' dom-test-reader true dom1 dom-read-reader ?true }t

\ Iterate and modify the tree

t{ dom1 dom-document ?true }t

t{ dom1 dom-document? ?true }t

t{ dom1 dom-parent ?false }t

t{ dom1 dom-document ?true }t

t{ dom1 dom-children 4 ?s }t

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
t{ dom1 dom-get-value s" 1.1" ?str }t

t{ dom1 dom-next ?true dom.comment ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?false }t
t{ dom1 dom-children? ?false }t

t{ dom1 dom-get-value s"  This is a test file for the ffl-library " ?str }t

t{ s"  This is a (modified) test file " dom1 dom-set }t
t{ dom1 dom-get-value s"  This is a (modified) test file " ?str }t


t{ dom1 dom-next ?true dom.element ?s }t

t{ dom1 dom-first?    ?false }t
t{ dom1 dom-last?     ?true  }t
t{ dom1 dom-children? ?true  }t

t{ dom1 dom-get-name s" TEST" ?str }t


t{ dom1 dom-child ?true dom.element ?s }t

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

t{ dom1 dom-get-name s" BOOKLET" ?str }t

t{ dom1 dom-child ?true dom.element ?s }t

t{ dom1 dom-next  ?true dom.element ?s }t

t{ dom1 dom-child ?true dom.element ?s }t
t{ dom1 dom-next  ?true dom.element ?s }t

t{ dom1 dom-get-name s" FIRSTBOOK" ?str }t

t{ dom1 dom-remove ?true }t  \ remove first book
t{ dom1 dom-last?  ?true }t

t{ dom1 dom-parent ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.element ?s }t
t{ dom1 dom-next   ?true dom.cdata   ?s }t

t{ dom1 dom-get-value s\" Dutch: \"Forth, een praktische introduktie\"" ?str }t
t{ s" ToBeDeleted" dom1 dom-set }t
t{ dom1 dom-get-value s" ToBeDeleted" ?str }t
t{ dom1 dom-remove ?true }t

\ Write the tree

variable dom-test-counter  dom-test-counter 0!

: dom-test-writer   ( c-addr u x - flag )
  drop
  dom-test-counter @ CASE
    0 OF s\" <?xml version=\"1.1\" standalone=\"no\"?>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    1 OF s" <!-- This is a (modified) test file -->" compare 0= IF dom-test-counter 1+! THEN ENDOF
    2 OF s" <TEST><BOOKLET><TITLE>Starting Forth</TITLE>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    3 OF s" <AUTHOR><NAME>Leo Brodie</NAME>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    4 OF s" </AUTHOR>"  compare 0= IF dom-test-counter 1+! THEN ENDOF
    5 OF s" <PUBLISHER>Prentice-Hall</PUBLISHER>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    6 OF s" <DATE>&quot;1981&quot;</DATE>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    7 OF s" <PAGES>235</PAGES>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    8 OF s" <REVIEW>This book is the best!.</REVIEW>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    9 OF s" <AVAILABLE/>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    10 OF s\" <SUBJECT MAIN=\"Computers\" SUB=\"Programming\">Introduction to the forth programming language</SUBJECT>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    11 OF s" </BOOKLET>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    12 OF s" </TEST>" compare 0= IF dom-test-counter 1+! THEN ENDOF
    >r 2drop r>
  ENDCASE
  false 0= \ true
;


t{ 0 ' dom-test-writer dom1 dom-write-writer ?true }t

t{ dom-test-counter @ 13 ?s }t

t{ dom1 dom-free }t

t{ dom.file close-file throw }t


\ Building a (small) tree from scratch

\ t{ dom-new value dom2 }t
t{ dom-create dom2 }t

t{ dom.document dom2 dom-append-node }t

t{ s" version" s" 1.0" dom.attribute dom2 dom-append-node }t
t{ dom2 dom-parent ?true dom.document ?s }t

t{ s" TAG"  dom.element dom2 dom-append-node }t
t{ s" Test" dom.text    dom2 dom-append-node }t

t{ dom2 dom-parent ?true dom.element ?s }t
t{ dom2 dom-parent ?true dom.document ?s }t

t{ s"  End " dom.comment dom2 dom-append-node }t

t{ dom2 dom-write-string ?true s\" <?xml version=\"1.0\"?><TAG>Test</TAG><!-- End -->" ?str }t

\ ==============================================================================
