\ ==============================================================================
\
\    gsv19_expl - the gtk-server fixed container example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/gsv.fs
include ffl/act.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  act-create events               \ Tree for the events -> xt translation

  variable xpos  50 xpos !
  variable ypos  50 ypos !

  0 value fixed                   \ Widgets/events


  : widget>event   ( n -- c-addr u = Convert the widget id to an event string )
    s>d tuck dabs <# #s rot sign #>
  ;
  
  
  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  : deleted-event  ( widget u -- f = process the exit event for widget c-addr u )
    drop
    true                        \ Delete event -> stop
  ;


  : move-button   ( widget -- f = Process the button click for widget c-addr u )
    ypos @ 50 + 300 mod dup ypos !
    xpos @ 30 + 300 mod dup xpos !
    rot fixed gtk_fixed_move
    false
  ;
    

  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new >r
    
    ['] deleted-event r@ events act-insert  \ Insert the event/widget with the xt

    s" Fixed Container" r@ gtk_window_set_title

    10 r@ gtk_container_set_border_width

    gtk_fixed_new to fixed
    fixed r@ gtk_container_add
    fixed gtk_widget_show
  
    150 BEGIN
      ?dup
    WHILE
      \ Creates a new button with the label "Press me"
      s" Press me" gtk_button_new_with_label >r
  
      \ When the button receives the "clicked" signal, it will call the function move_button()
      ['] move-button r@ events act-insert
  
      \ This packs the button into the fixed containers window. 
      dup dup r@ fixed gtk_fixed_put
  
      \ The final step is to display this newly created widget.
      r> gtk_widget_show
      50 -
    REPEAT

    r> gtk_widget_show            \ window
    
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback  
      event>widget 
      dup events act-get IF       \ find the xt for the event and execute it
        execute
      ELSE
        2drop false
      THEN
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
