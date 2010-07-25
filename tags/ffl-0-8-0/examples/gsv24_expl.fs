\ ==============================================================================
\
\     gsv24_expl - the gtk-server button boxes example in the ffl
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


  0 value window                  \ Widgets/events
  
  

  : create_bbox    ( n4 n3 n2 n1 f c-addr u -- = Create a bbox with a title c-addr u, horizontal flag, layout n1, spacing n2, child heigh n3, child width n4 )
    gtk_frame_new >r

    IF
      gtk_hbutton_box_new
    ELSE
      gtk_vbutton_box_new
    THEN
    >r

    5 r@ gtk_container_set_border_width
    r@ r'@ gtk_container_add

    \ Set the appearance of the Button Box 
    r@ gtk_button_box_set_layout
    r@ gtk_box_set_spacing
    
    \ r@ gtk_button_box_set_child_size
    2drop

    s" gtk-ok" gtk_button_new_from_stock
    r@ gtk_container_add

    s" gtk-cancel" gtk_button_new_from_stock
    r@ gtk_container_add

    s" gtk-help" gtk_button_new_from_stock
    r> gtk_container_add

    r>
  ;


  : example
    gtk_init                      \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Button Boxes" window gtk_window_set_title

    10 window gtk_container_set_border_width

    0 false gtk_vbox_new >r
    r@ window gtk_container_add

    s" Horizontal Button Boxes" gtk_frame_new >r
    10 true true r@ r'@ gtk_box_pack_start

    0 false gtk_vbox_new >r 
    10 r@ gtk_container_set_border_width
    r@ r'@ gtk_container_add

    85 20 40 GTK_BUTTONBOX_SPREAD true s" Spread (spacing 40)" create_bbox >r
    0 true true r> r@ gtk_box_pack_start

    85 20 30 GTK_BUTTONBOX_EDGE true s" Edge (spacing 30)" create_bbox >r
    5 true true r> r@ gtk_box_pack_start

    85 20 20 GTK_BUTTONBOX_START true s" Start (spacing 20)" create_bbox >r
    5 true true r> r@ gtk_box_pack_start

    85 20 10 GTK_BUTTONBOX_END true s" End (spacing 10)" create_bbox >r
    5 true true r> r@ gtk_box_pack_start

    rdrop rdrop                   \ vbox and frame
    
    s" Vertical Button Boxes" gtk_frame_new >r
    10 true true r@ r'@ gtk_box_pack_start

    0 false gtk_hbox_new >r
    10 r@ gtk_container_set_border_width
    r@ r'@ gtk_container_add

    85 20 5 GTK_BUTTONBOX_SPREAD false s" Spread (spacing 5)" create_bbox >r
    0 true true r> r@ gtk_box_pack_start

    85 20 30 GTK_BUTTONBOX_EDGE false s" Edge (spacing 30)" create_bbox >r 
    5 true true r> r@ gtk_box_pack_start

    85 20 20 GTK_BUTTONBOX_START false s" Start (spacing 20)" create_bbox >r
    5 true true r> r@ gtk_box_pack_start

    85 20 20 GTK_BUTTONBOX_END false s" End (spacing 20)" create_bbox >r
    5 true true r> r@ gtk_box_pack_start

    rdrop rdrop                   \ hbox and frame
    rdrop                         \ main_vbox

    window gtk_widget_show_all

                         
    BEGIN                         \ main loop
      s" WAIT" gtk_server_callback
      event>widget window =       \ Only one event
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
