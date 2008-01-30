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
\  $Date: 2008-01-30 06:54:00 $ $Revision: 1.5 $
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


( dom = XML Document Object Model )
( The dom module implements a simplified XML Document Object Model. The   )
( module reads a XML source into a tree of nodes. The tree can then be    )
( iterated and modified. After modification the tree can be written to a  )
( XML destination. As with every DOM the tree can take a lot of memory    )
( for large XML documents. DTD are not stored in the tree. Depending on   )
( the node type the following stack state is expected c.q. returned by    )
( the iterator:                                                           )
( <pre>                                                                   )
( dom.element        -- c-addr u              = Tag name                                            )
( dom.attribute      -- c-addr1 u1 c-addr2 u2 = Attribute name c-addr1 u1 and value c-addr2 u2      )
( dom.text           -- c-addr u              = Normal xml text                                     )
( dom.cdata          -- c-addr u              = CDATA section text                                  )
( dom.pi             -- c-addr u              = Proc. instr. target c-addr1 u1 and value c-addr2 u2 )
( dom.comment        -- c-addr n              = Comment                                             )
( dom.document       --                       = Document attributes                                 )
( </pre>                                                                  )

1 constant dom.version


( XML node types )

begin-enumeration
  enum: dom.not-used           ( -- n  = Not used       )
  enum: dom.element            ( -- n  = Tag            )
  enum: dom.attribute          ( -- n  = Attribute      )
  enum: dom.text               ( -- n  = Text           )
  enum: dom.cdata              ( -- n  = CDATA          )
  enum: dom.entity-ref         ( -- n  = Not used       )
  enum: dom.entity             ( -- n  = Not used       )
  enum: dom.pi                 ( -- n  = Proc. Instr.   )
  enum: dom.comment            ( -- n  = Comment        )
  enum: dom.document           ( -- n  = Start document )
  enum: dom.doc-type           ( -- n  = Not used       )
  enum: dom.doc-fragment       ( -- n  = Not used       )
  enum: dom.notation           ( -- n  = Not used       )
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
  dup dom.element <> swap dom.document <> AND IF
    r@ dom>node>value str-set
  THEN
  r@ dom>node>type @
  dup dom.element = over dom.attribute = OR swap dom.pi = OR IF
    r@ dom>node>name str-set
  THEN
  rdrop
;


: dom-node-init   ( c-addr1 u1 c-addr2 u2 +n dom-node -- = Initialise the DOM node with value c-addr1 u1, name c-addr2 u2 and type n2 )
  >r
  r@ dom>nnn        nnn-init
  r@ dom>node>type  !        \ node.type  = type
  r@ dom>node>name  str-init \ node.name  = empty
  r@ dom>node>value str-init \ node.value = emtpy
  r> dom-node-set
;


: dom-node-new   ( c-addr1 u1 c-addr2 u2 +n -- dom-node = Create a new DOM node with value c-addr1 u1, name c-addr2 u2 and type n2 )
  dom%node% allocate throw  >r r@ dom-node-init r>
;


: dom-node-free   ( dom%node -- = Free the DOM node from the heap )
  \ nnn ?
  \ dup dom>node>name str-(free)
  \ dup dom>node>value str-(free)
  free throw
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


: dom-create       ( "<spaces>name" -- ; -- dom = Create a named DOM in the dictionary )
  create   here   dom% allot   dom-init
;


: dom-new          ( -- dom = Create a new DOM on the heap )
  dom% allocate  throw  dup dom-init
;


: dom-free         ( dom -- = Free the DOM from the heap )
  \ nni -> ok
  ['] dom-node-free over dom>tree nnt-execute  \ free all xml nodes in the tree
  \ tos-(free)
  free throw 
;


( Iterating the DOM tree )

: dom-get-type   ( dom -- n true | false = Get the xml node type of the current node )
  dom>iter nni-get 
  dup nil<> IF
    dom>node>type @
    true
  ELSE
    drop
    false
  THEN
;


: dom-get-name   ( dom -- c-addr u true | false = Get the name from the current node )
  dom>iter nni-get
  dup nil<> IF
    dom>node>name str-get
    true
  ELSE
    drop
    false
  THEN
;


: dom-get-value   ( dom -- c-addr u true | false = Get the value from the current node )
  dom>iter nni-get
  dup nil<> IF
    dom>node>value str-get
    true
  ELSE
    drop
    false
  THEN
;


: dom-document   ( dom -- true | false = Move the iterator to the document [=root] node )
  dom>iter nni-root nil<>
;


: dom-document?   ( dom -- flag = Check if the current node is the document [=root] node )
  dom>iter nni-root?
;

: dom-parent   ( dom -- n true | false = Move the iterator to the parent node, return the xml type of this node )
  dom>iter nni-parent dom-get-type
;


: dom-children   ( dom -- n = Return the number of children for the current node )
  dom>iter nni-children
;


: dom-children?   ( dom -- flag = Check if the current node has children )
  dom>iter nni-children?
;


: dom-child   ( dom -- n true | false = Move the iterator to the first child node, return the xml type of this node )
  dom>iter nni-child dom-get-type
;


: dom-first   ( dom -- n true | false = Move the iterator to the first sibling node, return the xml type of this node )
  dom>iter nni-first dom-get-type
;


: dom-first?   ( dom -- flag = Check if the current node is the first sibling node )
  dom>iter nni-first?
;


: dom-next   ( dom -- n true | false = Move the iterator to the next sibling node, return the xml type of this node )
  dom>iter nni-next dom-get-type
;


: dom-prev   ( dom -- n true | false = Move the iterator to the previous sibling node, return the xml type of this node )
  dom>iter nni-prev dom-get-type
;


: dom-last   ( dom -- n true | false = Move the iterator to the last sibling node, return the xml type of this node )
  dom>iter nni-last dom-get-type
;


: dom-last?   ( dom -- flag = Check if the current node is the last sibling node )
  dom>iter nni-last?
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
  nni+throw
  dom-node-set
;


: dom-append-node   ( i*x n dom -- = Append a node to the current node, exception if not allowed, iterator is moved to the new node )
  \ ToDo: check valid
  >r
  dom-node-new
  r> dom>iter nni-append-child
  
;


: dom-insert-node-before   ( i*x n dom -- = Insert a node before the current node, exception if not allowed, iterator is moved to the new node )
  \ ToDo: check valid
  >r
  dom-node-new
  r> dom>iter nni-insert-before
;


: dom-insert-node-after   ( i*x n -- = Insert a node after the current node, exception if not allowed, iterator is moved to the new node )
  \ ToDo: check valid
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
      xis.empty-element OF dom.element  r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.cdata         OF dom.cdata    r@ dom-append-node                          r@ dom-parent 2drop ENDOF
      xis.proc-instr    OF dom.pi       r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.internal-dtd  OF 2drop 2drop                                                            ENDOF
      xis.public-dtd    OF 2drop 2drop 2drop 2drop                                                ENDOF
      xis.system-dtd    OF 2drop 2drop 2drop                                                      ENDOF
    ENDCASE
  REPEAT
  rdrop
  swap xis-free
  xis.done =
;


: dom-throw-full   ( dom -- = Throw an invalid state exception if the tree is full )
  dom>tree nnt-empty? 0= exp-invalid-state AND throw
;


( Reading the DOM tree )

: dom-read-string   ( c-addr u dom -- flag = Read xml source from the string c-addr u into the dom tree, throw exception if tree is not empty )
  >r
  r@ dom-throw-full
  xis-new
  >r
  true r@ xis-strip!              \ setup the xml parser
       r@ xis-set-string
  r> r>
  dom-read
;


: dom-read-reader   ( x xt dom -- flag = Read xml source with the reader xt with its state x into the dom tree, throw exception if tree is not empty )
  >r
  r@ dom-throw-full
  xis-new
  >r
  true r@ xis-strip!              \ setup the xml parser
       r@ xis-set-reader
  r> r>
  dom-read
;


( Private writing words )

defer dom.write-nodes

: dom-fetch-attributes  ( dom -- c-addr1 u1 .. c-addr2n u2n n = Fetch the attributes from the xml tree )
  0                               \ Attribute counter
  over dom-get-type
  BEGIN
    IF
      dom.attribute =             \ While attributes
    ELSE
      false
    THEN
  WHILE
    over dom-get-name  drop 2swap \ Fetch name and value, increment counter
    over dom-get-value drop 2swap
    1+
    over dom-next                 \ Move to the next node
  REPEAT
  nip
;


: dom-write-element   ( dom -- = Write the element c-addr u )
  >r
  r@ dom-get-name
  r@ dom-child IF
    drop
    r@ -rot 2>r dom-fetch-attributes 2r>   \ Fetch the attributes after saving the element name
      
    r@ dom>xos xos-write-start-tag         \ Write the start tag with its attributes
    r@ dom.write-nodes                     \ Write the child nodes of the element
    r@ dom-parent
      dom.element <> exp-invalid-state AND throw  \ Move to the parent (element) and check this
    r@ dom-get-name r@ xos-write-end-tag   \ Finish with the end tag
  ELSE
    0 r@ dom>xos xos-write-empty-element
  THEN
  rdrop
;


: dom-write-pi   ( dom -- = Write the proc. instr. c-addr u )
  >r
  r@ dom-get-name
  r@ dom-child IF
    drop
    r@ -rot 2>r dom-fetch-attributes 2>r    \ Fetch the attributes after saving the element name
    r@ dom>xos xos-write-proc-instr
    r@ dom-parent 
      dom.pi <> exp-invalid-state AND throw \ Move to the parent (proc. instr.) and check this
  ELSE
    0 r@ dom>xos xos-write-empty-element
  THEN
  rdrop
;


: dom-write-nodes   ( dom -- = Write the current sibling nodes )
  >r
  r@ dom-get-type
  BEGIN
  WHILE
    CASE
      dom.element   OF r@ dom-write-element                          ENDOF
      dom.pi        OF r@ dom-write-pi                               ENDOF
      
      dom.text      OF r@ dom-get-value r@ dom>xos xos-write-text    ENDOF
      dom.cdata     OF r@ dom-get-value r@ dom>xos xos-write-cdata   ENDOF
      dom.comment   OF r@ dom-get-value r@ dom>xos xos-write-comment ENDOF
    ENDCASE
    r@ dom-next  
  REPEAT
  rdrop
;

' dom-write-nodes is dom.write-nodes


( Writing the DOM tree )

: dom-write-string   ( dom -- c-addr u true | false = Write xml returning the string c-addr u if succesfull )
  >r
  r@ dom-document IF
    r@ dom-child ~~ IF
      drop
      r@ dom-fetch-attributes
      r@ dom>xos xos-write-start-xml
      r@ dom-write-nodes
    ELSE
      0
      r@ dom>xos xos-write-start-xml
    THEN
    r@ dom>xos str-get
    true
  ELSE
    false
  THEN
  rdrop
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
    dom.pi          OF ." PI       : " dup dom>node>value str-get type cr ENDOF
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
