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
\  $Date: 2008-06-25 16:48:34 $ $Revision: 1.19 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nfe.version [IF]

include ffl/nfs.fs
include ffl/chr.fs
include ffl/chs.fs


( nfe = Non-deterministic finite automata expression )
( The nfe module implements an expression in a non-deterministic finite      )
( automata. An expression is a concatenation, repetition or alteration of     )
( non-deterministic finite automata states [nfs]. An not yet fully built     )
( expression consists of two cells on the stack: a list with the non resolved)
( out states and a list of [nfs] states.<br>                                 )
( The code is based on the Thompson NFA algorithm published by Russ Cox.     )


2 constant nfe.version


( Expression structure )

begin-structure nfe%       ( -- n = Get the required space for a nfe expression )
  field: nfe>expression \ the expression: a list of states
  field: nfe>states     \ the number of states in the expression
  field: nfe>level      \ the paren level in the expression
  field: nfe>visit      \ the visit number
  field: nfe>string     \ the string the expression is matched against
  field: nfe>length     \ the length of the string
  field: nfe>index      \ the index in the string during matching
  field: nfe>char       \ the current character from the string
  field: nfe>icase      \ is the match case insensitive ?
  field: nfe>thread1    \ the first thread
  field: nfe>thread2    \ the second thread
  field: nfe>current    \ the current thread
  field: nfe>next       \ the next thread
  field: nfe>matches    \ the match result
end-structure


( Expression creation, initialisation and cleanup )

: nfe-init         ( nfe -- = Initialise the expression )
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


: nfe+free-expression   ( nfe -- = Free all states in the [sub]expression [recursive] )
  dup nil<> IF
    dup nfs-visit@ -1 <> IF            \ If start <> nil and not yet visited Then
      -1 over nfs-visit!               \   Set visited
      dup nfs-out1@ recurse            \   Recurse the out1 branch
      dup nfs-out2@ recurse            \   Recurse the out2 branch
      dup nfs-type@ nfs.class = IF     \   If state is a class Then
        dup nfs-data@ chs-free         \     Free the character set
      THEN
      dup nfs-free                     \   Free the state
    THEN
  THEN
  drop
;


: nfe-(free)   ( nfe -- = Free the internal, private variables from the heap )
  dup nfe>expression @ nfe+free-expression
  
  dup nfe>thread1 @ ?free throw
  dup nfe>thread2 @ ?free throw
      nfe>matches @ ?free throw
;


: nfe-create   ( "<spaces>name" -- ; -- nfe = Create a named expression in the dictionary )
  create   here   nfe% allot   nfe-init
;


: nfe-new   ( -- nfe = Create a new expression on the heap )
  nfe% allocate  throw  dup nfe-init
;


: nfe-free   ( nfe -- = Free the expression from the heap )
  dup nfe-(free) 
  
  free throw
;


( Member words )

: nfe-visit++   ( nfe -- n = Increment the visit number in the expression, return the visit number )
  nfe>visit dup @
  1+ 1 max 
  swap !
;


: nfe-level+@   ( nfe -- n = Increment and return the paren level )
  nfe>level dup 1+! @
;


: nfe-visit@   ( nfe -- n = Get the current visit number )
  nfe>visit @
;


: nfe-expression@   ( nfe -- a-addr = Get the list of states in the expression or nil )
  nfe>expression @
;


: nfe-states@   ( nfe -- n = Get the number of states in the expression )
  nfe>states @
;


: nfe-parens@   ( nfe -- n = Get the number of parens in the expression )
  nfe>level @ 1+
;


( Private matches words )

\ Matches
\   offset start: 0
\   offset end  : cell
\ Size : parens * (2 * cells)
    
: nfe+clear-matches   ( a-addr n -- = Clear n matches starting from a-addr )
  0 DO
    -1 over !            \ matches.start = -1
    cell+
    -1 over !            \ matches.end   = -1
    cell+
  LOOP
  drop
;


: nfe+copy-matches   ( a-addr1 addr2 n -- = Copy n matches from a-addr1 to a-addr2 )
  2* cells char/
  move
;


: nfe+match@   ( a-addr n1 -- n2 n3 = Get the nth match for a-addr, return the ep n2 and sp n3)
  2* cells + 2@
;


: nfe+match!   ( n1 n2 a-addr n3 -- = Set for the n3th match starting from a-addr the ep n1 and sp n2 )
  2* cells + 2!
;


: nfe+dump-matches ( a-addr1 n -- a-addr2 = Dump n matches, starting from a-addr1, return the next match a-addr2 )
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
    
: nfe-new-threads   ( nfe -- a-addr = Create a new thread array on the heap )
  dup  nfe-parens@ 2* 1+      \ thread size: parens * 2 + 1 ..
  swap nfe-states@ * cells    \ * states cells ..
  cell+                       \ + length
  allocate throw
  dup 0!                      \ length = 0
;


: nfe-start-next   ( nfe -- = Start using the next thread array )
  nfe>next @  0!
;


: nfe-current-length@   ( nfe -- n = Get the length of the current thread array )
  nfe>current @ @
;


: nfe-get-current   ( n nfe -- a-addr nfs = Get the nth matches a-addr and nfs state in the current thread array )
  tuck
  nfe-parens@ 2* 1+ cells     \ size thread element
  *                           \ * offset
  cell+                       \ + length field
  swap nfe>current @ +        \ address current thread element
  dup cell+                   \ matches
  swap @                      \ nfs state
;


: nfe-add-next   ( a-addr nfs nfe -- = Add the nfs state with the matches a-addr to the next thread array [recursive] )
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


: nfe-dump-threads   ( a-addr nfe -- = Dump the thread array )
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

: nfe+resolve   ( nfs1 nfs2 -- = Resolve all out states nfs2 to nfs1 )
  BEGIN
    dup nil<>         \ while outs <> nil do
  WHILE
    over swap @!      \   next = [outs]; [outs] = nfs
  REPEAT
  2drop
;


: nfe-new-nfs  ( x n nfe -- nfs = Create a new nfs state in the expression  with data x and type n )
  nfe>states dup 
  1+! @ nfs-new                \ create new nfs record with data, type and id
;
  
  
( Expression building words )

: nfe-clear   ( nfe -- = Clear the expression )
  dup nfe>thread1 @ ?free throw
  dup nfe>thread2 @ ?free throw
  dup nfe>matches @ ?free throw
  
  dup nfe-expression@ nfe+free-expression
  
      nfe-init
;
    
    
: nfe-single   ( x n nfe -- nfs1 nfs2 = Start an expression, nfs2 nfs1, with a single new state nfs1 with data x and type n )
  nfe-new-nfs             \ new nfs with data, type and id
  dup                     \ start = nfs
  nfs>out1 dup nil!       \ outs = nfs.out1
  swap
;


: nfe-concat   ( nfs1 nfs2 nfs3 nfs4 nfe -- nfs5 nfs6 = Concat the two expressions, return the outs nfs5 and start nfs6 )
  drop
  rot >r                 \ outs = outs2
  rot nfe+resolve        \ resolve outs -> start2
  r>                     \ start = start1
;


: nfe-paren   ( nfs1 nfs2 n nfe -- nfs3 nfs4 = Paren the expression with level n, return the new outs nf3 and start nfs4 )
  2dup 
  nfs.lparen swap nfe-new-nfs  \ new state '(' with level
  >r
  rot r@ nfs-out1!             \ '('.out1 = start
  nfs.rparen swap nfe-new-nfs  \ new state ')'
  tuck swap nfe+resolve        \ resolve outs -> ')'
  nfs>out1 dup nil!            \ outs = ')'.out1
  r>                           \ start = '('
;


: nfe-alternation   ( nfs1 nfs2 nfs3 nfs4 nfe -- nfs5 nfs6 = Make an alternation [|] of two expressions, return the new outs nfs5 and start nfs6 )
  nil nfs.split rot
  nfe-new-nfs               \ new split state
  >r   r@ nfs-out2!         \ split.out1 = start2
  swap r@ nfs-out1!         \ split.out2 = start1
  tuck                      \ append outs2 to outs1
  BEGIN
    dup @ dup nil<>
  WHILE
    nip
  REPEAT
  drop !
  r>                        \ start = split state, outs = outs1 + outs2
;


: nfe-zero-or-one   ( nfs1 nfs2 nfe -- nfs3 nfs4 = Repeat the expression one or zero [?] times, return the new start outs nfs3 and start nfs4 )
  nil nfs.split rot
  nfe-new-nfs              \ new split state
  tuck nfs-out1!           \ split.out1 = start (lazy: nfs-out2)
  tuck nfs>out2            \ split.out2 -> outs (lazy: nfs>out1)
  tuck !
  swap                     \ start = split, outs = split.out2
;  


: nfe-zero-or-more   ( nfs1 nfs2 nfe -- nfs3 nfs4 = Repeat the expression zero or more [*] times, return the new outs nfs3 and start nfs4 )
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  tuck nfs-out1!          \ split.out1 = start (lazy: nfs-out2)
  tuck swap nfe+resolve   \ resolve outs -> split
  dup
  nfs>out2 dup nil!       \ outs = split.out2 (lazy: nfs-out1)
  swap                    \ start = split
;


: nfe-one-or-more   ( nfs1 nfs2 nfe -- nfs3 nfs4 = Repeat the expression one or more [+] times, return the new outs nfs3 and start nfs4 )
  swap >r
  nil nfs.split rot
  nfe-new-nfs             \ new split state
  r@ over nfs-out1!       \ split.out1 = start (lazy: nfs-out2)
  tuck swap nfe+resolve   \ resolve outs -> split
  nfs>out2 dup nil!       \ outs = split.out2 (lazy: nfs-out1)
  r>                      \ start = start
;


: nfe-close   ( nfs1 nfs2 nfe -- nfs3 = Close the expression by adding the match state, return the start nfs3 )
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

: nfe-switch-threads   ( nfe -- = Switch current and next thread lists )
  dup  nfe>next    @
  over nfe>current @!
  swap nfe>next     !
;


: nfe-init-matching   ( c-addr u flag nfe -- = Initialise the matching with the string c-addr u and the case insensitive flag )
  tuck nfe>icase   !       \ save the case insensitive matching indication
  tuck nfe>length  !       \ save the to be matched string
       nfe>string  !
  
;


: nfe-start-matching   ( u nfe -- = Restart the matching from offset u in the string )
  >r 
  r@ nfe>index !         \ Set the start offset in the string
  
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


: nfe-setup-char    ( nfe -- = Setup the expression for matching the current character )
  dup  nfe>index @ chars
  over nfe>string @ + 
  c@                     \ Read the character from the string
  over nfe>icase @ IF
    chr-upper            \ Make upper case if case insensitive match
  THEN
  over nfe>char !
  nfe>index 1+!          \ Increment the string index
;


: nfe-step   ( nfe -- = Match the current character to the current list of nfs states )
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
        = 
        ENDOF
      
      nfs.any OF                   \   If any Then
        true                       \     Match
        ENDOF
        
      nfs.class OF                 \   If class Then
        r@ nfe>char @ over nfs-data@ chs-ch? \ Check if the character is in the set
        r@ nfe>icase @ IF          \     If case insensitive, check also the lowercase char
          over nfs-data@
          r@ nfe>char @ chr-lower
          swap chs-ch? OR
        THEN
        ENDOF
        
      nfs.match OF                 \   If match Then
        over r@ nfe>matches @ r@ nfe-parens@ nfe+copy-matches \ Copy matches from state in matches
        2>r drop dup 2r>           \      Ready
        false
        ENDOF
        
        false swap                 \ Default: no match
    ENDCASE
        
    IF                             \ If match Then
      nfs-out1@ r@ nfe-add-next    \    Put out1 state in the next thread list
    ELSE
      2drop
    THEN   
    1+
  REPEAT
  2drop
  rdrop
;


: nfe-stop-matching   ( nfe -- flag = Stop the matching of a string, return match result )
  dup nfe>char 0!
  dup nfe-step               \ Do the final step with character 0
      nfe>matches @ @ -1 <>  \ Check matches[0].start for an actual match
;


: nfe-steps   ( nfe -- = Perform all steps for matching )
  >r
  BEGIN
    r@ nfe>index @  r@ nfe>length @ <       \ While still chars in string ..
    r@ nfe-current-length@         0> AND   \ .. and states in the current thread Do
  WHILE
    r@ nfe-setup-char                       \   Read a character
    r@ nfe-step                             \   Check the current thread against the char
    r@ nfe-switch-threads                   \   Switch the threads
  REPEAT
  rdrop
;


( Matching words )

: nfe-match?  ( c-addr u flag nfe -- flag = Match a string c-addr u, with the flag indicating case insensitive match, return the match result )
  >r
  r@ nfe-init-matching         \ Initialise the match with the string and case info
  0 r@ nfe-start-matching      \ Start matching on string index 0
  r@ nfe-steps                 \ Do the matching
  r> nfe-stop-matching         \ Finish the matching
;


: nfe-search   ( c-addr u flag nfe -- n = Search in the string c-addr u for a match, with the flag indicating case insensitive match, return the first offset for a match, or -1 for no match )
  >r
  r@ nfe-init-matching
  -1
  r>
  dup nfe>length @ 0 ?DO
    I over nfe-start-matching
    dup nfe-steps
    dup nfe-stop-matching IF
      swap drop I swap
      LEAVE
    THEN
  LOOP
  drop
;


: nfe-result   ( n1 nfe -- n2 n3 = Get the match result of the n1th grouping, return match start n3 and end n2 )
  tuck nfe-parens@ index2offset
  swap
  2dup nfe-parens@ 0 swap within
  0= exp-index-out-of-range AND throw
  nfe>matches @ swap nfe+match@
;


( Private inspection words )

: nfe+dump-out   ( nfs -- = Dump the first out pointer )
  ." ->"        
  nfs-out1@ nfs-id@ 0 .r 
;


: nfe+dump   ( n nfs -- = Dump the expression starting from nfs, use visit number n for visit check [recursive] )
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

: nfe-dump  ( nfe -- = Dump the expression )
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

