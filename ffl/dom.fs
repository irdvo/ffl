\ ==============================================================================
\
\          dom - the XML Document object model module in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public
\ License as published by the Free Software Foundation; either
\ version 3 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ Lesser General Public License for more details.
\
\ You should have received a copy of the GNU Lesser General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2008-11-23 06:48:53 $ $Revision: 1.14 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] dom.version [IF]

include ffl/enm.fs
include ffl/nnn.fs
include ffl/nni.fs
include ffl/nnt.fs
include ffl/str.fs
include ffl/msc.fs
include ffl/xis.fs
include ffl/xos.fs


( dom = XML Document Object Model )
( The dom module implements a simplified XML Document Object Model. The   )
( module reads a XML source into a tree of nodes. The tree can then be    )
( iterated and modified. After modification the tree can be written to a  )
( XML destination. As with every DOM implementation the tree will use a   )
( lot of memory for large XML documents. Keep in mind that tree           )
( modifications checks are limited. DTDs are not stored in the tree.      )
( Depending on the node type the following stack state is expected by     )
( dom-set, dom-append-node, dom-insert-node-before and                    )
( dom-insert-node-after:                                                  )
( <pre>                                                                   )
( dom.element:   -- c-addr u              = Tag name                                       )
( dom.attribute: -- c-addr1 u1 c-addr2 u2 = Attribute name c-addr1 u1 and value c-addr2 u2 )
( dom.text:      -- c-addr u              = Normal xml text                                )
( dom.cdata:     -- c-addr u              = CDATA section text                             )
( dom.pi:        -- c-addr u              = Proc. instr. target c-addr u                   )
( dom.comment:   -- c-addr n              = Comment                                        )
( dom.document:  --                       = Document root                                  )
( </pre>                                                                  )

1 constant dom.version


( XML node types )

begin-enumeration
  enum: dom.not-used           ( -- n  = DOM node: Not used       )
  enum: dom.element            ( -- n  = DOM node: Tag            )
  enum: dom.attribute          ( -- n  = DOM node: Attribute      )
  enum: dom.text               ( -- n  = DOM node: Text           )
  enum: dom.cdata              ( -- n  = DOM node: CDATA          )
  enum: dom.entity-ref         ( -- n  = DOM node: Entity reference [not used] )
  enum: dom.entity             ( -- n  = DOM node: Entitiy [not used]          )
  enum: dom.pi                 ( -- n  = DOM node: Processing Instruction )
  enum: dom.comment            ( -- n  = DOM node: Comment        )
  enum: dom.document           ( -- n  = DOM node: Start document )
  enum: dom.doc-type           ( -- n  = DOM node: Document type [not used]     )
  enum: dom.doc-fragment       ( -- n  = DOM node: Document fragment [not used] )
  enum: dom.notation           ( -- n  = DOM node: Notation [not used]          )
end-enumeration


( Private DOM node structure )

begin-structure dom%node%   ( -- n = Get the required space for a dom node )
  nnn%
  +field  dom>nnn             \ the xml node extends the tree node
  field:  dom>node>type       \ the type of the node
  str%
  +field  dom>node>name       \ the name of the node
  str%
  +field  dom>node>value      \ the value of the node
end-structure


( Private DOM node creation, initialisation and destruction )

: dom-node-set    ( i*x dom-node -- = Update the DOM node )
  >r
  r@ dom>node>type @ 
  dup dom.element <> over dom.pi <> AND swap dom.document <> AND IF
    r@ dom>node>value str-set
  THEN
  r@ dom>node>type @
  dup dom.element = over dom.attribute = OR swap dom.pi = OR IF
    r@ dom>node>name str-set
  THEN
  rdrop
;


: dom-node-init   ( i*x +n dom-node -- = Initialise the DOM node with type n and optional name and value )
  >r
  r@ dom>nnn        nnn-init
  r@ dom>node>type  !        \ node.type  = type
  r@ dom>node>name  str-init \ node.name  = empty
  r@ dom>node>value str-init \ node.value = emtpy
  r> dom-node-set
;


: dom-node-new   ( i*x +n -- dom-node = Create a new DOM node with type n and optional name and value )
  dom%node% allocate throw  >r r@ dom-node-init r>
;


: dom-node-free   ( dom%node -- = Free the DOM node from the heap )
  dup dom>node>name  str-(free)
  dup dom>node>value str-(free)
  
  nnn-free
;
  

( DOM structure )

begin-structure dom%       ( -- n = Get the required space for a dom variable )
  nnt%
  +field  dom>tree            \ the DOM extends a tree
  nni%
  +field  dom>iter            \ the iterator on the tree
  tos%
  +field  dom>xos             \ the xml output stream
end-structure


( DOM creation, initialisation and destruction )

: dom-init         ( dom -- = Initialise the DOM )
  dup dom>tree nnt-init
  dup dom>tree over dom>iter nni-init
      dom>xos  tos-init
;


: dom-(free)       ( dom -- = Free the internal, private variables from the heap )
  dup dom>xos tos-(free)
  
  ['] dom-node-free swap dom>tree nnt-(free)
;


: dom-create       ( "<spaces>name" -- ; -- dom = Create a named DOM in the dictionary )
  create   here   dom% allot   dom-init
;


: dom-new          ( -- dom = Create a new DOM on the heap )
  dom% allocate  throw  dup dom-init
;


: dom-free         ( dom -- = Free the DOM from the heap )
  \ nni -> ok
  dup dom-(free)             \ Free the internal, private variables from the heap

  free throw                 \ Free the dom
;


( Private iterating words )

: dom-node-type   ( dom%node -- n true | false = Get the xml node type of the node )
  nil<>? IF
    dom>node>type @
    true
  ELSE
    false
  THEN
;


( Iterating the DOM tree )

: dom-get   ( dom -- n true | false = Get the xml node type of the current node )
  dom>iter nni-get dom-node-type
;
  
  
: dom-get-type   ( dom -- n = Get the xml node type of the current node )
  dom>iter nni-get 
  nil<>? IF
    dom>node>type @
  ELSE
    exp-invalid-state throw
  THEN
;


: dom-get-name   ( dom -- c-addr u = Get the name from the current node )
  dom>iter nni-get
  nil<>? IF
    dom>node>name str-get
  ELSE
    exp-invalid-state throw
  THEN
;


: dom-get-value   ( dom -- c-addr u = Get the value from the current node )
  dom>iter nni-get
  nil<>? IF
    dom>node>value str-get
  ELSE
    exp-invalid-state throw
  THEN
;


: dom-document   ( dom -- true | false = Move the iterator to the document [=root] node )
  dom>iter nni-root nil<>
;


: dom-document?   ( dom -- flag = Check if the current node is the document [=root] node )
  dom>iter nni-root?
;

: dom-parent   ( dom -- n true | false = Move the iterator to the parent node, return the xml type of this node )
  dom>iter nni-parent dom-node-type
;


: dom-children   ( dom -- n = Return the number of children for the current node )
  dom>iter nni-children
;


: dom-children?   ( dom -- flag = Check if the current node has children )
  dom>iter nni-children?
;


: dom-child   ( dom -- n true | false = Move the iterator to the first child node, return the xml type of this node )
  dom>iter nni-child dom-node-type
;


: dom-first   ( dom -- n true | false = Move the iterator to the first sibling node, return the xml type of this node )
  dom>iter nni-first dom-node-type
;


: dom-first?   ( dom -- flag = Check if the current node is the first sibling node )
  dom>iter nni-first?
;


: dom-next   ( dom -- n true | false = Move the iterator to the next sibling node, return the xml type of this node )
  dom>iter nni-next dom-node-type
;


: dom-prev   ( dom -- n true | false = Move the iterator to the previous sibling node, return the xml type of this node )
  dom>iter nni-prev dom-node-type
;


: dom-last   ( dom -- n true | false = Move the iterator to the last sibling node, return the xml type of this node )
  dom>iter nni-last dom-node-type
;


: dom-last?   ( dom -- flag = Check if the current node is the last sibling node )
  dom>iter nni-last?
;


( Private modify check words )

: dom-append-node?  ( n dom -- = Throws invalid state if appending of n to the current node is not allowed )
  swap
  dup dom.document = IF
    drop dom>tree nnt-empty? 0=                  \ append document, only if tree is empty
  ELSE
    dup dom.attribute = IF                       \ append attribute only for document, element or proc. instr.
      drop dom-get-type dup dom.document <> over dom.element <> AND swap dom.pi <> AND
    ELSE
      dup dom.element = over dom.text = OR over dom.cdata = OR over dom.comment = OR swap dom.pi = OR IF
        dom-get-type dup dom.document <> swap dom.element <> AND \ append ... only for document and element
      ELSE
        drop true
      THEN
    THEN
  THEN
  exp-invalid-state AND throw
;


: dom-insert-before?  ( n dom -- = Throws invalid state if inserting n before the current node is not allowed )
  swap
  dup dom.attribute = IF
    drop dom-get-type dom.attribute <>           \ insert attribute only before another attribute
  ELSE
    dup dom.element = over dom.text = OR over dom.cdata = OR over dom.comment = OR swap dom.pi = OR IF
      dom-get-type dup dom.document = swap dom.attribute = OR  \ insert .. only before not an attribute or document
    ELSE
      drop true
    THEN
  THEN
  exp-invalid-state AND throw
;


: dom-next-attribute?  ( dom -- flag = Check if the next node is an attribute )
  dup dom-last? 0= IF             \ If not last node Then
    dup  dom-next drop            \   Move to the next
    dom.attribute =               \   Check for attribute
    swap dom-prev 2drop           \   Move back
  ELSE
    drop false
  THEN
;


: dom-insert-after?  ( n dom -- Throws invalid state if inserting n after the current node is not allowed )
  swap
  dup dom.attribute = IF
    drop dom-get-type dom.attribute <>         \ insert attribute only after another attribute
  ELSE
    dup dom.element = over dom.text = OR over dom.cdata = OR over dom.comment = OR swap dom.pi = OR IF
      dup  dom-next-attribute?
      swap dom-get-type dom.document = OR      \ insert ... only after not an attribute or a document
    ELSE
      drop true
    THEN
  THEN
  exp-invalid-state AND throw
;


( Modifying the DOM tree )

\ Current   New
\           element attribute text cdata pi  comment document
\ element   AI      A         AI   AI    AI  AI      
\ attribute         I  
\ text      I                 I    I     I   I 
\ cdata     I                 I    I     I   I 
\ pi        I       A                    I   I 
\ comment   I                            I   I 
\ document  A       A                    A   A
\ A = append-node I = insert-before/after

: dom-set   ( i*x dom -- = Update the current node )
  dom>iter nni-get
  nni+nil-throw
  dom-node-set
;


: dom-append-node   ( i*x n dom -- = Append a node to the current node, exception if not allowed, iterator is moved to the new node )
  2dup dom-append-node?
  >r
  dom-node-new
  r> dom>iter nni-append-child
  
;


: dom-insert-node-before   ( i*x n dom -- = Insert a node before the current node, exception if not allowed )
  2dup dom-insert-before?
  >r
  dom-node-new
  r> dom>iter nni-insert-before
;


: dom-insert-node-after   ( i*x n -- = Insert a node after the current node, exception if not allowed )
  2dup dom-insert-after?
  >r
  dom-node-new
  r> dom>iter nni-insert-after
;


: dom-remove        ( dom -- flag = Remove the current sibling node without children from the tree, iterator is moved to the next, previous or parent node, return the removed node )
  dom>iter nni-remove nil<>
;


( Private read words )

: dom-append-attributes   ( c-addr2n u2n .. c-addr1 u1 n dom -- = Add all attributes to the element ) 
  swap 0 ?DO
    >r
    dom.attribute r@ dom-append-node
    r@ dom-parent 2drop
    r>
  LOOP
  drop
;


: dom-read   ( xis dom -- flag = Read the xml source into the dom tree, xis is freed )
  >r
  BEGIN
    dup xis-read
    dup xis.error <> over xis.done <> AND
  WHILE
    CASE
      xis.start-xml     OF dom.document r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.comment       OF dom.comment  r@ dom-append-node                          r@ dom-parent 2drop ENDOF
      xis.text          OF dom.text     r@ dom-append-node                          r@ dom-parent 2drop ENDOF
      xis.start-tag     OF dom.element  r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.end-tag       OF 2drop                                                    r@ dom-parent 2drop ENDOF
      xis.empty-element OF dom.element  r@ dom-append-node r@ dom-append-attributes r@ dom-parent 2drop ENDOF
      xis.cdata         OF dom.cdata    r@ dom-append-node                          r@ dom-parent 2drop ENDOF
      xis.proc-instr    OF dom.pi       r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.internal-dtd  OF 4drop                                                                  ENDOF
      xis.public-dtd    OF 4drop 4drop                                                            ENDOF
      xis.system-dtd    OF 4drop 2drop                                                            ENDOF
    ENDCASE
  REPEAT
  rdrop
  swap xis-free
  xis.done =
;


( Reading the DOM tree )

: dom-read-string   ( c-addr u flag1 dom -- flag2 = Read xml source from the string c-addr u into the dom tree, flag1 indicates whitespace stripping, throw exception if tree is not empty, return success in flag2 )
  >r
  xis-new
  >r
  r@ xis-strip!
  r@ xis-set-string
  r> r>
  dom-read
;


: dom-read-reader   ( x xt flag1 dom -- flag2 = Read xml source with the reader xt with its state x into the dom tree, flag1 indicates whitespace stripping, throw exception if tree is not empty, return success in flag2 )
  >r
  xis-new
  >r
  r@ xis-strip!
  r@ xis-set-reader
  r> r>
  dom-read
;


( Private writing words )

defer dom.write-nodes

: dom-fetch-attributes  ( dom -- c-addr1 u1 .. c-addr2n u2n n = Fetch the attributes from the xml tree )
  0                               \ Attribute counter
  over dom-get
  BEGIN
    IF
      dom.attribute =             \ While attributes
    ELSE
      false
    THEN
  WHILE
    over dom-get-name  2swap      \ Fetch name and value, increment counter
    over dom-get-value 2swap
    1+
    over dom-next                 \ Move to the next node
  REPEAT
  nip
;


: dom-write-element   ( dom -- = Write the current element )
  >r
  r@ dom-get-name
  r@ dom-child IF                          \ If Childs
    drop
    r@ -rot 2>r dom-fetch-attributes 2r>   \   Fetch the attributes after saving the element name
    r@ dom>xos xos-write-start-tag         \   Write the start tag with its attributes

    r@ dom.write-nodes                     \   Write the child nodes of the element

    r@ dom-parent 2drop                    \   Move to the parent (element) node

    r@ dom-get-name r@ dom>xos xos-write-end-tag  \ Finish with the end tag
  ELSE                                     \ Else
    0 -rot r@ dom>xos xos-write-empty-element  \ Write the empty element  
    
    r@ dom-parent 2drop                    \   Move to the parent (element) node
  THEN
  r> dom>xos tos-flush
;


: dom-write-pi   ( dom -- = Write the proc. instr. c-addr u )
  >r
  r@ dom-get-name
  r@ dom-child IF
    drop
    r@ -rot 2>r dom-fetch-attributes 2>r    \ Fetch the attributes after saving the element name
    r@ dom>xos xos-write-proc-instr

    r@ dom-parent drop 
      dom.pi <> exp-invalid-state AND throw \ Move to the parent (proc. instr.) and check this
  ELSE
    0 r@ dom>xos xos-write-empty-element
  THEN
  rdrop
;


: dom-write-nodes   ( dom -- = Write the current sibling nodes )
  >r
  r@ dom-get
  BEGIN
  WHILE
    \ r@ dom>xos str-get type cr
    CASE
      dom.element   OF r@ dom-write-element                          ENDOF
      dom.pi        OF r@ dom-write-pi                               ENDOF
      
      dom.text      OF r@ dom-get-value r@ dom>xos xos-write-text    ENDOF
      dom.cdata     OF r@ dom-get-value r@ dom>xos xos-write-cdata   ENDOF
      dom.comment   OF r@ dom-get-value r@ dom>xos xos-write-comment r@ dom>xos tos-flush ENDOF
    ENDCASE
    r@ dom-next  
  REPEAT
  rdrop
;
' dom-write-nodes is dom.write-nodes


: dom-write   ( dom -- flag = Write the tree )
  >r
  r@ dom-document dup IF
    r@ dom-child IF
      drop
      r@ dom-fetch-attributes
      r@ dom>xos xos-write-start-xml
      r@ dom>xos tos-flush
      r@ dom-write-nodes
    ELSE
      0
      r@ dom>xos xos-write-start-xml
      r@ dom>xos tos-flush
    THEN
  THEN  
  rdrop
;


( Writing the DOM tree )

: dom-write-string   ( dom -- c-addr u true | false = Write the tree to xml returning a string c-addr u if successful )
  >r
  r@ dom-write IF
    r@ dom>xos str-get
    true
  ELSE
    false
  THEN
  rdrop
;


: dom-write-writer   ( x xt dom -- flag = Write the tree to xml using writer xt and its data x, flag indicate success )
  >r
  r@ dom>xos tos-set-writer
  r> dom-write
;


( Private inspection word )

: dom-emit-node   ( dom%node -- = Emit the contents of the dom node )
  dup dom>node>type @
  CASE
    dom.document    OF ." Document." cr   ENDOF
    dom.element     OF ." Element  : " dup dom>node>name  str-get type cr ENDOF
    dom.attribute   OF ." Attribute: " dup dom>node>name  str-get type [char] = emit dup dom>node>value str-get type cr ENDOF
    dom.text        OF ." Text     : " dup dom>node>value str-get type cr ENDOF
    dom.cdata       OF ." CDATA    : " dup dom>node>value str-get type cr ENDOF
    dom.pi          OF ." PI       : " dup dom>node>name  str-get type cr ENDOF
    dom.comment     OF ." Comment  : " dup dom>node>value str-get type cr ENDOF
  ENDCASE
  drop
;


( Inspection )

: dom-dump   ( dom - = Dump the DOM tree )
  dom>tree ['] dom-emit-node swap nnt-execute
;
  
[THEN]

\ ==============================================================================

