\ ==============================================================================
\
\           nfe - the non-deterministic finite automata expression
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
\  $Date: 2007-05-22 19:40:05 $ $Revision: 1.10 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfe.version [IF]

include ffl/nfs.fs


( nfe = Non-deterministic finite automata expression )
( The nfe module implements an expression in a non-deterministic finite      )
( automata. An expression is a concatenation, repeation or alteration of     )
( non-deterministic finite automato states [nfs]. An not yet fully built     )
( expression consists of two cells on the stack: a list with the non resolved)
( out states and a list of [nfs] states.                                     )


1 constant nfe.version


( Expression structure )

struct: nfe%       ( - n = Get the required space for the nfe data structure )
  cell: nfe>expression \ the expression: a list of states
  cell: nfe>states     \ the number of states in the expression
  cell: nfe>level      \ the paren level in the expression
  cell: nfe>visit      \ the visit number
  cell: nfe>string     \ the string the expression is matched against
  cell: nfe>length     \ the length of the string
  cell: nfe>index      \ the index in the string during matching
  cell: nfe>case       \ is the match case sensitive ?
  cell: nfe>thread1    \ the first thread
  cell: nfe>thread2    \ the second thread
  cell: nfe>current    \ the current thread
  cell: nfe>next       \ the next thread
  cell: nfe>matches    \ the match result
;struct

  
( Expression creation, initialisation and cleanup )

: nfe-init         ( w:nfe - = Initialise the expression )
  dup nfe>expression nil!
  dup nfe>states       0!
  dup nfe>visit        0!
  dup nfe>level        0!
  dup nfe>string     nil!
  dup nfe>length       0!
  dup nfe>index        0!
  dup nfe>case        off
  dup nfe>thread1    nil!
  dup nfe>thread2    nil!
  dup nfe>current    nil!
  dup nfe>next       nil!
      nfe>matches    nil!
;


: nfe-create   ( C: "name" - R: - w:nfe = Create a named expression in the dictionary )
  create   here   nfe% allot   nfe-init
;


: nfe-new   ( - w:nfe = Create a new expression on the heap )
  nfe% allocate  throw  dup nfe-init
;


: nfe+free-expression   ( w:start - = Free all states in a [sub]expression [recursive] )
  dup nil<> IF
    dup nfs-visit@ -1 <> IF     \ If start <> nil and not yet visited Then
      -1 over nfs-visit!        \   Set visited
      dup nfs-out1@ recurse     \   Recurse the out1 branch
      dup nfs-out2@ recurse     \   Recurse the out2 branch
      dup nfs-free              \   Free the state
    THEN
  THEN
  drop
;


: nfe-free   ( w:nfe - = Free the nfe structure from the heap )
  dup nfe>expression @ nfe+free-expression
  
  dup nfe>thread1 @ ?free
  dup nfe>thread2 @ ?free
  dup nfe>matches @ ?free
  
  free throw
;


( Member words )

: nfe-visit+@   ( w:nfe - n = Get the next visit number )
  nfe>visit dup
  1+! @
;


: nfe-visit@   ( w:nfe - n = Get the current visit number )
  nfe>visit @
;


: nfe-expression@   ( w:nfe - w:list = Get the list of states in the expression or nil )
  nfe>expression @
;


: nfe-states@   ( w:nfe - n = Get the number of states in the expression )
  nfe>states @
;


: nfe-parens@   ( w:nfe - n = Get the number of parens in the expression )
  nfe>level @ 1+
;


( Private matches words )

: nfe+match@   ( w:matches n:offset - n:ep n:sp = Get the offsetth match )
  2* cells + 2@
;


: nfe+match!   ( n:ep n:sp w:matches n:offset - = Set the offsetth match )
  2* cells + 2!
;


( Private matching thread words )

\ Dynamic array of thread structures:
\    offset length: 0
\    offset states: cell
\    offset state : cell + index * ( cell + parens * 2 * cells )
\ Size: cell + states * ( cell + parens * 2 * cells )
    
: nfe-new-threads   ( w:nfe - w:threads = Create a new thread array on the heap )
  dup  nfe-parens@ 2* 1+      \ thread size: parens * 2 + 1 ..
  swap nfe-states@ * cells    \ * states cells ..
  cell+                       \ + length
  allocate throw
  dup 0!                      \ length = 0
;


: nfe-start-threads   ( w:nfe - = Start using the next thread array )
  nfe>next @  0!
;


: nfe-threads-length@   ( w:nfe - n:length = Get the length of the current thread array )
  nfe>current @ @
;


: nfe-get-thread   ( n:offset w:nfe - w:matches w:nfs = Get the offsetth matches and nfs state in the current thread array )
  tuck
  nfe-parens@ 2* 1+ cells     \ size thread element
  *                           \ * offset
  cell+                       \ + length field
  swap nfe>current @ +        \ address current thread element
  dup cell+                   \ matches
  swap @                      \ nfs state
;


: nfe-add-thread   ( w:matches w:nfs w:nfe - = Add the nfs state with the matches to the next thread array [recursive] )
  >r 
  dup nfs-visit@ r@ nfe-visit@ <> IF
    r@ nfe-visit@ over nfs-visit!          \ Set this state visited
    
    2dup                                   \ Setup for storing
    
    r@ nfe>next @ dup @ swap 1+!           \ fetch the offset and increase
    
    r@ nfe-parens@                         \ offset > addr
    2* 1+ cells * cell+
    r@ nfe>next @ +
    
    tuck !                                 \ thread.state
    cell+                                  \ thread.matches
    r@ nfe-parens@ 2* cells 1 chars /
    move                                   \ move the matches from the state in the thread
    
    dup nfs-type@
    CASE                                   \ if split then
      nfs.split OF 
        2dup nfs-out1@ r@ recurse          \   add the states of out1 and out2
        2dup nfs-out2@ r@ recurse 
        ENDOF
      nfs.lparen OF                        \ if ( then
        2dup  nfs-data@ nfe+match@         \   save the match state of this paren
        2over -1 r@ nfe>index @ 
        2swap nfs-data@ nfe+match!         \   set the start index 
        2over nfs-out1@ r@ recurse         \   add the out state
        2over nfs-data@ nfe+match!         \   restore the match state of this paren
        ENDOF
      nfs.rparen OF                        \ if ) then
        2dup  nfs-data@ nfe+match@         \   save the match state of this paren
        2over 2over nip r@ nfe>index @ swap 
        2swap nfs-data@ nfe+match!         \   set the end index 
        2over nfs-out1@ r@ recurse         \   add the out state
        2over nfs-data@ nfe+match!         \   restore the match state of this paren
        ENDOF
    ENDCASE
  THEN
  2drop
  rdrop
;


: nfe-dump-threads   ( w:threads w:nfe - = Dump the thread array )
  over nil<> IF
    nfe-parens@ swap
    dup cell+ swap                    \ S: parens addr
    @ 0 ?DO                           \ Do for all states in thread
      dup @ nfs-id@ 0 .r              \   Print state id
      cell+
      [char] : emit
      over 0 ?DO                      \   Do for all matches for state
        [char] ( emit
        dup @ 0 .r cell+              \     Print start
        [char] , emit
        dup @ 0 .r cell+              \     Print end
        [char] ) emit
      LOOP
      [char] ; emit
    LOOP
  THEN
  2drop
;


( Private expression building words )

: nfe+resolve   ( w:nfs w:outs - = Resolve all out states to nfs )
  BEGIN
    dup nil<>             \ while out <> nil do
  WHILE
    2dup  nfs-out1@       \   out = out.out1
    2swap nfs-out1!       \   out.out1 = nfs
  REPEAT
  2drop
;


: nfe-new-nfs  ( w:data n:type w:nfe - w:nfs = Create a new nfs in the expression )
  nfe>states dup 
  1+! @ nfs-new                \ create new nfs record with data, type and id
;
  
  
: nfe-nparen   ( w:outs w:start n:level w:nfe - w:outs w:start = Paren the expression with paren level n)
  2dup 
  nfs.lparen swap nfe-new-nfs  \ new state '(' with level
  >r
  rot r@ nfs-out1!             \ '('.out1 = start
  nfs.rparen swap nfe-new-nfs  \ new state ')'
  tuck swap nfe+resolve        \ outs = ')' start = '('
  r>
;


( Expression building words )

: nfe-clear   ( w:nfe - = Clear the expression )
  dup nfe>thread1 @ ?free
  dup nfe>thread2 @ ?free
  dup nfe>matches @ ?free
  
  dup nfe-expression@ nfe+free-expression
  
      nfe-init
;
    
    
: nfe-single   ( w:data w:type w:nfe - w:outs w:start = Start an expression with a single new state )
  nfe-new-nfs             \ new nfs with data, type and id
  dup                     \ start = nfs, outs = nfs
;


: nfe-concat   ( w:outs1 w:start1 w:outs2 w:start2 w:nfe - w:outs w:start = Concat the two expressions )
  drop
  rot >r                 \ outs = outs2
  rot nfe+resolve        \ resolve outs1 -> start2
  r>                     \ start = start1
;


: nfe-paren   ( w:outs w:start w:nfe - w:outs w:start = Paren the expression )
  dup nfe>level dup 1+! @   \ the next paren level
  swap nfe-nparen           \ paren the expression with this level
;


: nfe-alternation   ( w:outs2 w:start2 w:outs1 w:start1 w:nfe - w:outs w:start = Make an alternation [|] of two expressions )
  nil nfs.split rot
  nfe-new-nfs               \ new split state
  >r   r@ nfs-out1!         \ split.out1 = start1
  swap r@ nfs-out2!         \ split.out2 = start2
  tuck                      \ append outs2 to outs1
  BEGIN
    dup nfs-out1@ dup nil<>
  WHILE
    nip
  REPEAT
  drop nfs-out1!
  r>                        \ start = split state, outs = outs1 + outs2
;


: nfe-zero-or-one   ( w:outs w:start w:nfe - w:outs w:start = Repeat the expression one or zero [?] times )
  nil nfs.split rot
  nfe-new-nfs              \ new split state
  tuck nfs-out2!           \ split.out2 = start
  tuck nfs-out1!           \ split.out1 = outs
  dup                      \ outs = outs + split, start = split   
;  


: nfe-zero-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe zero or more [*] times )
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  tuck nfs-out2!          \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  dup                     \ outs = split, start = split
;


: nfe-one-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe one or more [+] times )
  swap >r
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  r@ over nfs-out2!       \ split.out2 = start
  tuck swap nfe+resolve   \ resolve outs -> split
  r>                      \ outs = split, start = start
;


: nfe-close   ( w:outs w:start w:nfe - w:start = Close the expression by adding the match state )
  >r
  0 r@ nfe-nparen         \ paren the full expression with level 0
  nil nfs.match 
  r@ nfe-new-nfs          \ new match state
  rot nfe+resolve         \ resolve outs -> match
  
  r@ nfe>expression !     \ save the expression
  
  r@ nfe-new-threads      \ create ..
  r@ nfe>thread1 !        \ .. thread1
  r@ nfe-new-threads      \ create ..
  r@ nfe>thread2 !        \ .. thread2
  
  r@ nfe-parens@ 2* cells \ create ..
  allocate throw          \ .. the ..
  r> nfe>matches !        \ .. matches array
;


( Private matching words )

: nfe-switch-threads   ( w:nfe - = Switch current and next thread lists )
  dup  nfe>next    @
  over nfe>current @!
  swap nfe>next     !
;


: nfe-start   ( c-addr u f:case w:nfe - = Start the matching )
  >r
  r@ nfe>case    !       \ save the case sensitive matching indication
  r@ nfe>length  !       \ save the to be matched string
  r@ nfe>string  !
  r@ nfe>index  0!
                         \ setup the thread lists
  r@ nfe>thread1 @ r@ nfe>current !
  r@ nfe>thread2 @ r@ nfe>next    !
                         \ init matches
  r@ nfe>matches @                       
  r@ nfe-parens@ 0 DO
    -1 over !            \ matches.start = -1
    cell+
    -1 over !            \ matches.end   = -1
    cell+
  LOOP
  drop
                         \ increase the visit number
  r@ nfe-visit+@ drop
                         \ start using the next threads
  r@ nfe-start-threads
                         \ add the start nfs to the thread list
  r@ nfe>matches @ r@ nfe>expression @ r@ nfe-add-thread
                         
  r> nfe-switch-threads                        
;


: nfe-step   ( w:nfe - = Match a single character to the current list of nfs states )
  \ ToDo
;

( Matching words )

: nfe-match   ( c-addr u f:case w:nfe - false | matches true = Match a string )
  >r
  r@ nfe-start
  \ ToDo
  rdrop
;


( Private inspection words )

: nfe+dump-out   ( w:nfs - = Dump the first out pointer )
  ." ->"        
  nfs-out1@ nfs-id@ 0 .r 
;


: nfe+dump   ( n:visit w:start - = Dump the expression [recursive] )
  dup nil<> IF
    2dup nfs-visit@ <> IF         \ If start <> nil and not yet visited Then
      2dup nfs-visit!             \   Set visited
      dup nfs-id@ 0 .r            \   Show id, type, optional data and nexts
      [char] : emit
      dup nfs-type@ 
      CASE
        nfs.char   OF 
          dup nfs-data@ emit 
          dup nfe+dump-out
          ENDOF
        nfs.any    OF 
          [char] . emit
          dup nfe+dump-out
          ENDOF
        nfs.class  OF 
          [char] [ emit
          dup nfe+dump-out 
          ENDOF
        nfs.lparen OF 
          [char] ( emit 
          dup nfs-data@ 0 .r 
          dup nfe+dump-out 
          ENDOF
        nfs.rparen OF 
          [char] ) emit 
          dup nfs-data@ 0 .r 
          dup nfe+dump-out
          ENDOF
        nfs.split  OF 
          [char] ? emit
          dup nfe+dump-out
          [char] , emit
          dup nfs-out2@ nfs-id@ 0 .r 
          ENDOF
        nfs.match  OF 
          [char] ! emit 
          ENDOF
      ENDCASE
      [char] ; emit space
      2dup nfs-out1@ recurse      \   Recurse the out1 branch
      2dup nfs-out2@ recurse      \   Recurse the out2 branch
    THEN
  THEN
  2drop
;


( Inspection )

: nfe-dump  ( w:nfe - = Dump the expression )
  ." nfe:" dup . cr
  ."  states    :" dup nfe-states@ . cr
  ."  level     :" dup nfe>level   ? cr
  ."  visit     :" dup nfe>visit   ? cr
  ."  expression:" dup nfe-visit+@ over nfe-expression@ nfe+dump cr
  ."  string    :" dup nfe>string  ? cr
  ."  length    :" dup nfe>length  ? cr
  ."  index     :" dup nfe>index   ? cr
  ."  case      :" dup nfe>case    ? cr
  ."  thread1   :" dup nfe>thread1 ? cr
  ."  thread2   :" dup nfe>thread2 ? cr
  ."  current   :" dup nfe>current @ over nfe-dump-threads cr
  ."  next      :" dup nfe>next    @ over nfe-dump-threads cr
  ."  matches   :"     nfe>matches ? cr
;

[THEN]

\ ==============================================================================
 
