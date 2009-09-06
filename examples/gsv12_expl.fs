\ ==============================================================================
\
\     gsv12_expl - the gtk-server statusbar example in the ffl
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
include ffl/spf.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


    variable   count     
  1 count !
    str-create msg
    
  0 value      window             \ the widget/events
  0 value      push-button
  0 value      pop-button
  0 value      context_id
  0 value      status_bar
  
  
  : push_item      ( -- f = Push item on statusbar )
    count @ msg spf" Item %d"
    
    msg str-get context_id status_bar gtk_statusbar_push drop
    
    count 1+!
  ;


  : pop_item       ( -- f = Pop item from statusbar )
    context_id status_bar gtk_statusbar_pop
  ;


  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    100 200 window gtk_widget_set_size_request
    
    s" GTK Statusbar Example" window gtk_window_set_title
 
    1 false gtk_vbox_new >r
    r@ window gtk_container_add
          
    gtk_statusbar_new to status_bar
    0 true true status_bar r@ gtk_box_pack_start
    status_bar gtk_widget_show

    s" Statusbar example" status_bar gtk_statusbar_get_context_id to context_id

    s" push item" gtk_button_new_with_label to push-button
    
    2 true true push-button r@ gtk_box_pack_start
    
    push-button gtk_widget_show

    s" pop last item" gtk_button_new_with_label to pop-button
    
    2 true true pop-button r@ gtk_box_pack_start
    
    pop-button gtk_widget_show

    r>     gtk_widget_show        \ vbox
    window gtk_widget_show


                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        push-button  OF push_item false ENDOF
        pop-button   OF pop_item  false ENDOF
        false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
