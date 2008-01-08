\ ==============================================================================
\
\          dom - the XML Document object model module in the ffl
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
\  $Date: 2008-01-08 19:20:16 $ $Revision: 1.2 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dom.version [IF]

include ffl/enm.fs
include ffl/nnn.fs
include ffl/nni.fs
include ffl/nnt.fs
include ffl/str.fs
include ffl/xis.fs
include ffl/xos.fs


( dom = XML Document model )
( The dom module implements a simplified XML Document Object Model. The   )
( module reads a XML source into a tree of nodes. The tree can then be    )
( iterated and modified. After modification the tree can be written to a  )
( XML destination. As with every DOM the tree can take a lot of memory    )
( for large XML documents.                                                )

1 constant dom.version


( Private DOM node structure )

begin-enumeration        \ Description    Name      Value
  enum: dom.not-used           \ Not used       <emtpy>   <empty>
  enum: dom.element            \ Tag            name      <empty>
  enum: dom.attribute          \ Attribute      name      value
  enum: dom.text               \ Text           #text     text
  enum: dom.cdata              \ CDATA          #cdata    text
  enum: dom.entity-ref         \ Not used       <empty>   <empty>
  enum: dom.entity             \ Not used       <empty>   <empty>
  enum: dom.pi                 \ Proc. Instr.   name      value
  enum: dom.comment            \ Comment        #comment  text
  enum: dom.document           \ Start document #document value
  enum: dom.doc-type           \ Not used       <empty>   <empty>
  enum: dom.doc-fragment       \ Not used       <empty>   <empty>
  enum: dom.notation           \ Not used       <empty>   <empty>
end-enumeration


begin-structure dom%node%   ( -- n = Get the required space for a dom node )
  field:  dom>node>type       \ the type of the node
  str%
  +field  dom>node>name       \ the name of the node
  str%
  +field  dom>node>value      \ the value of the node
end-structure


( Private DOM node creation, initialisation and destruction )

: dom-node-set    ( i*x dom-node -- = Update the DOM node with value c-addr1 u1 and name c-addr2 u2 )
  \ ToDo
  >r
  r@ dom>node>type @ CASE
    dom.element   OF r@ dom>node>name str-set ENDOF
    dom.attribute OF r@ dom>node>name str-set r@ dom>node>value str-set ENDOF
    dom.text      OF s" #text" r@ dom>node>name str-set  r@ dom>node>value str-set ENDOF
    dom.cdata     OF s" #cdata" r@ dom>node>name str-set  r@ dom>node>value str-set ENDOF
    dom.pi        OF r@ dom>node>name str-set  r@ dom>node>value str-set ENDOF
    dom.comment   OF s" #comment" r@ dom>node>name str-set  r@ dom>node>value str-set ENDOF
    dom.document  OF s" #document" r@ dom>node>name str-set  r@ dom>node>value str-set ENDOF
  ENDCASE
  rdrop
;


: dom-node-init   ( c-addr1 u1 c-addr2 u2 +n dom-node -- = Initialise the DOM node with value c-addr1 u1, name c-addr2 u2 and type n2 )
  >r
  r@ dom>node>type !         \ node.type  = type
  r@ dom>node>name  str-init \ node.name  = empty
  r@ dom>node>value str-init \ node.value = emtpy
  r> dom-node-set
;


: dom-node-new   ( c-addr1 u1 c-addr2 u2 +n -- dom-node = Create a new DOM node with value c-addr1 u1, name c-addr2 u2 and type n2 )
  dom%node% allocate throw  >r r@ dom-node-init r>
;


: dom-node-free   ( dom-node -- = Free the DOM node from the heap )
  \ ToDo: free strings
  free throw
;
  

( DOM structure )

begin-structure dom%       ( -- n = Get the required space for a dom variable )
  nnt%
  +field  dom>tree            \ the DOM extends a tree
  nni%
  +field  dom>iter            \ the iterator on the tree
end-structure


( DOM creation, initialisation and destruction )

: dom-init         ( dom -- = Initialise the DOM )
  \ ToDo
;


: dom-create       ( "<spaces>name" -- ; -- dom = Create a named DOM in the dictionary )
  create   here   dom% allot   dom-init
;


: dom-new          ( -- dom = Create a new DOM on the heap )
  dom% allocate  throw  dup dom-init
;


: dom-free         ( dom -- = Free the DOM from the heap )
  \ ToDo
  free throw 
;


( Iterating the DOM tree )

: dom-document   ( dom -- flag = Move the iterator to the document [=root] node )
;


: dom-document?   ( dom -- flag = Check if the current node is the document [=root] node )
;


: dom-parent   ( dom -- i*x n true | false = Move the iterator to the parent node )
;


: dom-children   ( dom -- n = Return the number of children for the current node )
;


: dom-children?   ( dom -- n = Check if the current node has children )
;


: dom-child   ( dom -- i*x n true | false = Move the iterator to the first child node )
;


: dom-first   ( dom -- i*x n true | false = Move the iterator to the first sibling node )
;


: dom-first?   ( dom -- i*x n true | false = Check if the current node is the first sibling node )
;


: dom-next   ( dom -- i*x n true | false = Move the iterator to the next sibling node )
;


: dom-prev   ( dom -- i*x n true | false = Move the iterator to the previous sibling node )
;


: dom-last   ( dom -- i*x n true | false = Move the iterator to the last sibling node )
;


: dom-last?   ( dom -- i*x n true | false = Check if the current node is the last sibling node )
;


( Modifying the DOM tree )

\ Current   New
\           element attribute text cdata pi  comment document
\ element   AI      A         AI   AI    AI  AI      
\ attribute         I  
\ text      I                 I    I     I   I 
\ cdata     I                 I    I     I   I 
\ pi-0      I                            I   I 
\ pi-n      I                 I    I     I   I 
\ comment-0 I                            I   I 
\ comment-n I                 I    I     I   I 
\ document  A                            A   A
\ A = append-node I = insert-before/after

: dom-set   ( i*x -- = Update the current node )
;


: dom-append-node   ( i*x n -- = Append a node to the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-insert-node-before   ( i*x n -- = Insert a node before the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-insert-node-after   ( i*x n -- = Insert a node after the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-remove        ( dom -- i*x n = Remove the current sibling node without children from the tree, iterator is moved to the next, previous or parent node, return the removed node )
;


( Reading the DOM tree )

: dom-read-string   ( c-addr u dom -- flag = Read xml source from the string c-addr u )
;


: dom-read-reader   ( x xt dom -- flag = Read xml source with the reader xt with its state x )
;


( Writing the DOM tree )

: dom-write-string   ( c-addr u dom -- u true | false = Write xml to the string c-addr u )
;


: dom-write-writer   ( x xt dom -- flag = Write xml to the writer xt with its state x )
;


( Inspection )

: dom-dump   ( dom - = Dump the DOM tree )
;
  
[THEN]

\ ==============================================================================
