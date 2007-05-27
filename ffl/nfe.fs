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
\  $Date: 2007-05-27 10:02:13 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfe.version [IF]

include ffl/nfs.fs
include ffl/chr.fs


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
  cell: nfe>char       \ the current character from the string
  cell: nfe>icase      \ is the match case insensitive ?
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
  dup nfe>icase       off
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

: nfe-visit++   ( w:nfe - n = Increment the visit number )
  nfe>visit dup @
  1+ 1 max 
  swap !
;


: nfe-level+@   ( w:nfe - n = Increment and get the paren level )
  nfe>level dup 1+! @
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

\ Matches
\   offset start: 0
\   offset end  : cell
\ Size : parens * (2 * cells)
    
: nfe+clear-matches   ( w:matches n:number - = Clear number matches )
  0 DO
    -1 over !            \ matches.start = -1
    cell+
    -1 over !            \ matches.end   = -1
    cell+
  LOOP
  drop
;


: nfe+copy-matches   ( w:from w:to n:number - = Copy number matches )
  2* cells 
  1 chars /
  move
;


: nfe+match@   ( w:matches n:offset - n:ep n:sp = Get the offsetth match )
  2* cells + 2@
;


: nfe+match!   ( n:ep n:sp w:matches n:offset - = Set the offsetth match )
  2* cells + 2!
;


: nfe+dump-matches ( w:matches n:number - w:next = Dump number matches )
  0 ?DO                   \   Do for all matches 
    [char] ( emit
    dup @ 0 .r cell+      \     Print start
    [char] , emit
    dup @ 0 .r cell+      \     Print end
    [char] ) emit
  LOOP
;


( Private matching thread words )

\ Dynamic array of thread structures:
\    offset length: 0
\    offset states: cell
\    offset state : cell + index * ( cell + parens * 2 * cells )
\ Size: cell + states * ( cell + size matches )
    
: nfe-new-threads   ( w:nfe - w:threads = Create a new thread array on the heap )
  dup  nfe-parens@ 2* 1+      \ thread size: parens * 2 + 1 ..
  swap nfe-states@ * cells    \ * states cells ..
  cell+                       \ + length
  allocate throw
  dup 0!                      \ length = 0
;


: nfe-start-next   ( w:nfe - = Start using the next thread array )
  nfe>next @  0!
;


: nfe-current-length@   ( w:nfe - n:length = Get the length of the current thread array )
  nfe>current @ @
;


: nfe-get-current   ( n:offset w:nfe - w:matches w:nfs = Get the offsetth matches and nfs state in the current thread array )
  tuck
  nfe-parens@ 2* 1+ cells     \ size thread element
  *                           \ * offset
  cell+                       \ + length field
  swap nfe>current @ +        \ address current thread element
  dup cell+                   \ matches
  swap @                      \ nfs state
;


: nfe-add-next   ( w:matches w:nfs w:nfe - = Add the nfs state with the matches to the next thread array [recursive] )
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
    r@ nfe-parens@ nfe+copy-matches        \ copy the matches from the state in the thread
    
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
        2over nfs-out1@ r@ recurse         \   add the out1 state
        2over nfs-data@ nfe+match!         \   restore the match state of this paren
        ENDOF
      nfs.rparen OF                        \ if ) then
        2dup  nfs-data@ nfe+match@         \   save the match state of this paren
        2over 2over nip r@ nfe>index @ swap 
        2swap nfs-data@ nfe+match!         \   set the end index 
        2over nfs-out1@ r@ recurse         \   add the out1 state
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
      over nfe+dump-matches           \   Print matches
      [char] ; emit
    LOOP
  THEN
  2drop
;


( Private expression building words )

: nfe+resolve   ( w:nfs w:outs - = Resolve all out states to nfs )
  BEGIN
    dup nil<>         \ while outs <> nil do
  WHILE
    over swap @!      \   next = [outs]; [outs] = nfs
  REPEAT
  2drop
;


: nfe-new-nfs  ( w:data n:type w:nfe - w:nfs = Create a new nfs in the expression )
  nfe>states dup 
  1+! @ nfs-new                \ create new nfs record with data, type and id
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
  dup                     \ start = nfs
  nfs>out1 dup nil!       \ outs = nfs.out1
  swap
;


: nfe-concat   ( w:outs1 w:start1 w:outs2 w:start2 w:nfe - w:outs w:start = Concat the two expressions )
  drop
  rot >r                 \ outs = outs2
  rot nfe+resolve        \ resolve outs -> start2
  r>                     \ start = start1
;


: nfe-paren   ( w:outs w:start n:level w:nfe - w:outs w:start = Paren the expression )
  2dup 
  nfs.lparen swap nfe-new-nfs  \ new state '(' with level
  >r
  rot r@ nfs-out1!             \ '('.out1 = start
  nfs.rparen swap nfe-new-nfs  \ new state ')'
  tuck swap nfe+resolve        \ resolve outs -> ')'
  nfs>out1 dup nil!            \ outs = ')'.out1
  r>                           \ start = '('
;


: nfe-alternation   ( w:outs2 w:start2 w:outs1 w:start1 w:nfe - w:outs w:start = Make an alternation [|] of two expressions )
  nil nfs.split rot
  nfe-new-nfs               \ new split state
  >r   r@ nfs-out1!         \ split.out1 = start1
  swap r@ nfs-out2!         \ split.out2 = start2
  tuck                      \ append outs2 to outs1
  BEGIN
    dup @ dup nil<>
  WHILE
    nip
  REPEAT
  drop !
  r>                        \ start = split state, outs = outs1 + outs2
;


: nfe-zero-or-one   ( w:outs w:start w:nfe - w:outs w:start = Repeat the expression one or zero [?] times )
  nil nfs.split rot
  nfe-new-nfs              \ new split state
  tuck nfs-out1!           \ split.out1 = start (lazy: nfs-out2)
  tuck nfs>out2            \ split.out2 -> outs (lazy: nfs>out1)
  tuck !
  swap                     \ start = split, outs = split.out2
;  


: nfe-zero-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe zero or more [*] times )
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  tuck nfs-out1!          \ split.out1 = start (lazy: nfs-out2)
  tuck swap nfe+resolve   \ resolve outs -> split
  dup
  nfs>out2 dup nil!       \ outs = split.out2 (lazy: nfs-out1)
  swap                    \ start = split
;


: nfe-one-or-more   ( w:outs w:start w:nfe - w:outs w:start = Repeat nfe one or more [+] times )
  swap >r
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  r@ over nfs-out1!       \ split.out1 = start (lazy: nfs-out2)
  tuck swap nfe+resolve   \ resolve outs -> split
  nfs>out2 dup nil!       \ outs = split.out2 (lazy: nfs-out1)
  r>                      \ start = start
;


: nfe-close   ( w:outs w:start w:nfe - w:start = Close the expression by adding the match state )
  >r
  0 r@ nfe-paren          \ paren the full expression with level 0
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


: nfe-start-matching   ( c-addr u f:icase w:nfe - = Start the matching )
  >r
  r@ nfe>icase   !       \ save the case insensitive matching indication
  r@ nfe>length  !       \ save the to be matched string
  r@ nfe>string  !
  r@ nfe>index  0!
                         \ setup the thread lists
  r@ nfe>thread1 @ r@ nfe>current !
  r@ nfe>thread2 @ r@ nfe>next    !
  
                         \ clear matches
  r@ nfe>matches @ r@ nfe-parens@ nfe+clear-matches
                         
                         \ increase the visit number
  r@ nfe-visit++
                         \ start using the next threads
  r@ nfe-start-next
                         \ add the start nfs to the thread list
  r@ nfe>matches @ r@ nfe>expression @ r@ nfe-add-next
                         
  r> nfe-switch-threads                        
;


: nfe-setup-char ( w:nfe - = Setup the char in the nfe structure )
  dup  nfe>index @ chars
  over nfe>string @ + 
  c@                     \ Read the character from the string
  over nfe>icase @ IF
    chr-upper            \ Make upper case if case insensitive match
  THEN
  over nfe>char !
  nfe>index 1+!          \ Increment the string index
;


: nfe-step   ( w:nfe - = Match the current character to the current list of nfs states )
  >r
  r@ nfe-visit++                   \ Next visit number
  r@ nfe-start-next                \ Start using the next thread
  r@ nfe-current-length@ 0         \ Do for all states in the current thread list
  BEGIN
    2dup >
  WHILE
    dup r@ nfe-get-current         \   Get the thread in the current list
    
    dup nfs-type@                  \   Get the type of the state in the thread
    CASE
      nfs.char OF                  \   If char Then
        r@ nfe>char @ over nfs-data@   \     Compare the char with the string data
        r@ nfe>icase @ IF
          chr-upper 
        THEN
        = IF                       \     If match Then
          2dup nfs-out1@ r@ nfe-add-next  \ Put1 out state in the next thread list
        THEN
        ENDOF
      
      nfs.any OF                   \   If any Then
        2dup nfs-out1@ r@ nfe-add-next    \ Put out state in the next thread list
        ENDOF
        
      nfs.match OF                 \   If match Then
        over r@ nfe>matches @ r@ nfe-parens@ nfe+copy-matches \ Copy matches from state in matches
        2>r drop dup 2r>           \      Ready
        ENDOF
    ENDCASE
    2drop
    1+
  REPEAT
  2drop
  rdrop
;


: nfe-stop-matching   ( w:nfe - f = Stop the matching of a string )
  dup nfe>char 0!
  dup nfe-step               \ Do the final step with character 0
      nfe>matches @ @ -1 <>  \ Check matches[0].start for an actual match
;


( Matching words )

: nfe-match   ( c-addr u f:icase w:nfe - f = Match a string )
  >r
  r@ nfe-start-matching
  BEGIN
    r@ nfe>index @  r@ nfe>length @ <
    r@ nfe-current-length@         0> AND
  WHILE
    r@ nfe-setup-char
    r@ nfe-step
    r@ nfe-switch-threads
  REPEAT
  r> nfe-stop-matching
;


: nfe-match@   ( n:index w:nfe - n:ep n:sp = Get the match result of the indexth grouping )
  tuck nfe-parens@ index2offset
  swap
  2dup nfe-parens@ 0 swap within
  0= exp-index-out-of-range AND throw
  nfe>matches @ swap nfe+match@
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
  ."  expression:" dup nfe-visit++ dup nfe-visit@ over nfe-expression@ nfe+dump cr
  ."  string    :" dup nfe>string  ? cr
  ."  length    :" dup nfe>length  ? cr
  ."  index     :" dup nfe>index   ? cr
  ."  char      :" dup nfe>char   c@ emit cr
  ."  icase     :" dup nfe>icase   ? cr
  ."  thread1   :" dup nfe>thread1 ? cr
  ."  thread2   :" dup nfe>thread2 ? cr
  ."  current   :" dup nfe>current @ over nfe-dump-threads cr
  ."  next      :" dup nfe>next    @ over nfe-dump-threads cr
  ."  matches   :" dup nfe>matches @ swap nfe-parens@ nfe+dump-matches drop cr
;

[THEN]

\ ==============================================================================

