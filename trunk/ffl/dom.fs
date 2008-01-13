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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.4 $
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
( dom.pi             -- c-addr1 u1 c-addr2 u2 = Proc. instr. target c-addr1 u1 and value c-addr2 u2 )
( dom.comment        -- c-addr n              = Comment                                             )
( dom.document       -- c-addr u              = Document attributes                                 )
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
  r@ dom>node>type @ dom.element <> IF
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


( Private iterator words )

: dom-node-get    ( dom-node | nil - i*x n true | false = Get the xml info from the node )
  >r
  r@ nil<> IF
    r@ dom>node>type @
    dup dom.element = over dom.attribute = OR swap dom.pi = OR IF
      r@ dom>node>name str-get
    THEN
    r@ dom>node>type @ dom.element <> IF
      r@ dom>node>value str-get
    THEN
    true
  ELSE
    false
  THEN
  rdrop
;


( Iterating the DOM tree )

: dom-get   ( dom -- i*x n true | false = Get the xml info of the current node )
  dom>iter nni-get dom-node-get
;


: dom-document   ( dom -- c-addr u n true | false = Move the iterator to the document [=root] node, return the document info )
  dom>iter nni-root dom-node-get
;


: dom-document?   ( dom -- flag = Check if the current node is the document [=root] node )
  dom>iter nni-root?
;

: dom-parent   ( dom -- i*x n true | false = Move the iterator to the parent node, return the xml info of this node )
  dom>iter nni-parent dom-node-get
;


: dom-children   ( dom -- n = Return the number of children for the current node )
  dom>iter nni-children
;


: dom-children?   ( dom -- n = Check if the current node has children )
  dom>iter nni-children?
;


: dom-child   ( dom -- i*x n true | false = Move the iterator to the first child node, return the xml info of this node )
  dom>iter nni-child dom-node-get
;


: dom-first   ( dom -- i*x n true | false = Move the iterator to the first sibling node, return the xml info of this node )
  dom>iter nni-first dom-node-get
;


: dom-first?   ( dom -- flag = Check if the current node is the first sibling node )
  dom>iter nni-first?
;


: dom-next   ( dom -- i*x n true | false = Move the iterator to the next sibling node, return the xml info of this node )
  dom>iter nni-next dom-node-get
;


: dom-prev   ( dom -- i*x n true | false = Move the iterator to the previous sibling node, return the xml info of this node )
  dom>iter nni-prev dom-node-get
;


: dom-last   ( dom -- i*x n true | false = Move the iterator to the last sibling node, return the xml info of this node )
  dom>iter nni-last dom-node-get
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


: dom-append-node   ( i*x n -- = Append a node to the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-insert-node-before   ( i*x n -- = Insert a node before the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-insert-node-after   ( i*x n -- = Insert a node after the current node, exception if not allowed, iterator is moved to the new node )
;


: dom-remove        ( dom -- i*x n = Remove the current sibling node without children from the tree, iterator is moved to the next, previous or parent node, return the removed node )
;


( Private read words )

: dom-append-attributes   ( c-addr2n u2n .. c-addr1 u1 n dom -- = Add all attributes to the element ) 
  swap 0 ?DO
    >r
    dom.attribute r@ dom-append-node
                  r@ dom-parent
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
      xis.start-xml     OF dom.document r@ dom-append-node r@ dom-append-attributes r@ dom-parent ENDOF
      xis.comment       OF dom.comment  r@ dom-append-node                          r@ dom-parent ENDOF
      xis.text          OF dom.text     r@ dom-append-node                          r@ dom-parent ENDOF
      xis.start-tag     OF dom.element  r@ dom-append-node r@ dom-append-attributes               ENDOF
      xis.end-tag       OF                                                          r@ dom-parent ENDOF
      xis.empty-element OF dom.element  r@ dom-append-node r@ dom-append-attributes r@ dom-parent ENDOF
      xis.cdata         OF dom.cdata    r@ dom-append-node                          r@ dom-parent ENDOF
      xis.proc-instr    OF dom.pi       r@ dom-append-node r@ dom-append-attributes r@ dom-parent ENDOF
      xis.internal-dtd  OF 2drop 2drop                                                            ENDOF
      xis.public-dtd    OF 2drop 2drop 2drop 2drop                                                ENDOF
      xis.system-dtd    OF 2drop 2drop 2drop                                                      ENDOF
    ENDCASE
  REPEAT
  rdrop
  swap xis-free
  xis.done =
;


( Reading the DOM tree )

: dom-read-string   ( c-addr u dom -- flag = Read xml source from the string c-addr u into the dom tree )
  \ ToDo: test for empty tree
  >r
  xis-new
  >r
  true r@ xis-strip!              \ setup the xml parser
       r@ xis-set-string
  r> r>
  dom-read
;


: dom-read-reader   ( x xt dom -- flag = Read xml source with the reader xt with its state x into the dom tree )
  \ ToDo: test for empty tree
  >r
  xis-new
  >r
  true r@ xis-strip!              \ setup the xml parser
       r@ xis-set-reader
  r> r>
  dom-read
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
