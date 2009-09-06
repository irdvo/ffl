\ ==============================================================================
\
\       gsv6_expl - the gtk-server toggle button example in the ffl
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

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;

  0 value window                  \ the event widgets
  0 value exit-button
  0 value toggle1
  0 value toggle2
  0 value toggle3
  0 value group


  : toggle-clicked       ( widget -- = process the toggle event )
    dup gtk_toggle_button_get_active IF
      gtk_button_get_label type
      ."  is active" cr
    ELSE
      gtk_button_get_label type
      ."  is not active" cr
    THEN
  ;
  

  : example
    gtk_init                    \ Initialise toolkit
                                \ Create toplevel window and connect delete-event with word
    GTK_WINDOW_TOPLEVEL gtk_window_new to window

    s" Radio buttons" window gtk_window_set_title
  
    0 window gtk_container_set_border_width

    0 false gtk_vbox_new >r

    r@ window gtk_container_add
    
    10 false gtk_vbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    0 true true r@ r'@ gtk_box_pack_start
    
    r@ gtk_widget_show            \ box2

    s" Button1" 0 gtk_radio_button_new_with_label to toggle1     
    
    0 true true toggle1 r@ gtk_box_pack_start
    
    toggle1 gtk_widget_show

    toggle1 gtk_radio_button_get_group to group
    
    s" Button2" group gtk_radio_button_new_with_label to toggle2

    true toggle2 gtk_toggle_button_set_active
    
    0 true true toggle2 r@ gtk_box_pack_start
    
    toggle2 gtk_widget_show

    s" Button3" toggle2 gtk_radio_button_new_with_label_from_widget to toggle3
    
    0 true true toggle3 r> gtk_box_pack_start
    
    toggle3 gtk_widget_show

    gtk_hseparator_new >r
    0 true false r@ r'@ gtk_box_pack_start
    r> gtk_widget_show

    10 false gtk_vbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    0 true false r@ r'@ gtk_box_pack_start

    s" Close" gtk_button_new_with_label to exit-button
    
    0 true true exit-button r@ gtk_box_pack_start
    
    \ GTK_WIDGET_SET_FLAGS (button, GTK_CAN_DEFAULT); Not possible ?
    \ r@ gtk_widget_grab_default
    
    exit-button gtk_widget_show
    r>          gtk_widget_show   \ box3
    r>          gtk_widget_show   \ box1
    window      gtk_widget_show
    
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        exit-button  OF true ENDOF
        toggle1      OF toggle1 toggle-clicked false ENDOF
        toggle2      OF toggle2 toggle-clicked false ENDOF
        toggle3      OF toggle3 toggle-clicked false ENDOF
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
