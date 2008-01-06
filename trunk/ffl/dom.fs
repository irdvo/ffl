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
\  $Date: 2008-01-06 20:11:10 $ $Revision: 1.1 $
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

: dom-node-set    ( c-addr1 u1 c-addr2 u2 dom-node -- = Update the DOM node with value c-addr1 u1 and name c-addr2 u2 )
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

[THEN]

\ ==============================================================================
