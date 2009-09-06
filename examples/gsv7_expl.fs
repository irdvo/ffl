\ ==============================================================================
\
\       gsv7_expl - the gtk-server adjustment example in the ffl
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

  0 value window                  \ the widget events
  0 value exit-button
  0 value adj1
  0 value adj2
  0 value adj3
  0 value chk-btn
  0 value hscale
  0 value vscale
  0 value top-menu
  0 value bottom-menu
  0 value left-menu
  0 value right-menu
  0 value cont-menu
  0 value discont-menu
  0 value delayed-menu
  0 value hscale2
  0 value hscale3


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  : set-value-position  ( n -- )
    dup hscale gtk_scale_set_value_pos
        vscale gtk_scale_set_value_pos
  ;
  

  : set-update-policy ( n -- )
    dup hscale gtk_range_set_update_policy
        vscale gtk_range_set_update_policy
  ;

  
  : set-scale-digits
    adj2 gtk_adjustment_get_value
    
    f>d d>s dup
    hscale gtk_scale_set_digits
    vscale gtk_scale_set_digits
  ;

  
  : set-page-size  ( -- = set the page size )
    \ adj3 gtk_adjustment_get_value fdup

                  \ Set the page size and page increment size of the sample
                  \ adjustment to the value specified by the "Page Size" scale
    \ adj1 gtk_adjustment_set_page_size
    \ adj1 gtk_adjustment_set_page_increment
    
    \  This sets the adjustment and makes it emit the "changed" signal to 
    \  reconfigure all the widgets that are attached to this signal.  */
    \  gtk_adjustment_set_value (set, CLAMP (set->value,
		  \	    set->lower,
		  \	    (set->upper - set->page_size)));
    s" changed" adj1 gtk_signal_emit_by_name
  ;

  
  : set-draw-value
    chk-btn gtk_toggle_button_get_active
    
    dup hscale gtk_scale_set_draw_value
        vscale gtk_scale_set_draw_value
  ;


  : scale_set_default_values  ( widget -- = Set the default values for a scale widget )
    GTK_UPDATE_CONTINUOUS over gtk_range_set_update_policy
    1                     over gtk_scale_set_digits
    GTK_POS_TOP           over gtk_scale_set_value_pos
    true                  swap gtk_scale_set_draw_value
  ;


  : create_range_controls  ( -- = Create the range controls )

                                  \ Standard window-creating stuff
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Range controls" window gtk_window_set_title

    0 false gtk_vbox_new >r

    r@ window gtk_container_add

    
    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    0 true true r@ r'@ gtk_box_pack_start
    
        \ value, lower, upper, step_increment, page_increment, page_size */
        \ Note that the page_size value only makes a difference for
        \ scrollbar widgets, and the highest value you'll get is actually
        \ (upper - page_size). */
    1.0E+0 1.0E0 0.1E+0 101.0E+0 0.0E+0 0.0E+0 gtk_adjustment_new to adj1
  
    adj1 gtk_vscale_new to vscale
    
    vscale scale_set_default_values
    
    0 true true vscale r@ gtk_box_pack_start
    
    vscale gtk_widget_show


    10 false gtk_vbox_new >r
    
    0 true true r@ r'@ gtk_box_pack_start
    

                                  \ Reuse the same adjustment */
    adj1 gtk_hscale_new to hscale
    
    -1 200 hscale gtk_widget_set_size_request
    
    hscale scale_set_default_values
    
    0 true true hscale r@ gtk_box_pack_start
    
    hscale gtk_widget_show

                                  \ Reuse the same adjustment again
    adj1 gtk_hscrollbar_new >r
    
    GTK_UPDATE_CONTINUOUS r@ gtk_range_set_update_policy

    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

    r> gtk_widget_show            \ box3
    r> gtk_widget_show            \ box2
    
    
    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    0 true true r@ r'@ gtk_box_pack_start


                                  \ A checkbutton to control whether the value is displayed or not
    s" Display value on scale widgets" gtk_check_button_new_with_label to chk-btn
    
    true chk-btn gtk_toggle_button_set_active
        
    0 true true chk-btn r@ gtk_box_pack_start
    
    chk-btn gtk_widget_show
    
    r> gtk_widget_show            \ box2
  
    
    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width

                                  \ An option menu to change the position of the value
    s" Scale Value Position:" gtk_label_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

  
    gtk_option_menu_new >r
    
    gtk_menu_new >r 

    
    s" Top" gtk_menu_item_new_with_label to top-menu

    top-menu gtk_widget_show

    top-menu r@ gtk_menu_shell_append


    s" Bottom" gtk_menu_item_new_with_label to bottom-menu

    bottom-menu gtk_widget_show

    bottom-menu r@ gtk_menu_shell_append


    s" Left" gtk_menu_item_new_with_label to left-menu

    left-menu gtk_widget_show

    left-menu r@ gtk_menu_shell_append

  
    s" Right" gtk_menu_item_new_with_label to right-menu
    
    right-menu gtk_widget_show

    right-menu r@ gtk_menu_shell_append


    r> r@ gtk_option_menu_set_menu
    
    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ opt
  
    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ box2


    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width

                \ Yet another option menu, this time for the update policy of the scale widgets
    s" Scale Update Policy:" gtk_label_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    gtk_option_menu_new >r
    
    gtk_menu_new >r

    s" Continuous" gtk_menu_item_new_with_label to cont-menu

    cont-menu gtk_widget_show

    cont-menu r@ gtk_menu_shell_append


    s" Discontinuous" gtk_menu_item_new_with_label to discont-menu

    discont-menu gtk_widget_show

    discont-menu r@ gtk_menu_shell_append

  
    s" Delayed" gtk_menu_item_new_with_label to delayed-menu
    
    delayed-menu gtk_widget_show

    delayed-menu r@ gtk_menu_shell_append
  
    r> r@ gtk_option_menu_set_menu

    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ opt
    
  
    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width
  
              \ An HScale widget for adjusting the number of digits on the
              \  sample scales.
    s" Scale Digits:" gtk_label_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    0.0E+0 1.0E+0 1.0E+0 5.0E+0 0.0E+0 1.0E+0 gtk_adjustment_new to adj2
    
    \ Crashes the gtk-server:
    \ ['] cb_digits_scale s" cb_digits_scale" 
    \ 2dup s" value_changed" adj2 widget>event gtk_server_connect 2drop
    \ events hct-insert
    
    adj2 gtk_hscale_new to hscale2
    
    0 hscale2 gtk_scale_set_digits
    
    0 true true hscale2 r@ gtk_box_pack_start
    
    hscale2 gtk_widget_show

    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ box2 
  
  
    10 false gtk_hbox_new >r
    
    10 r@ gtk_container_set_border_width
  
                    \ And, one last HScale widget for adjusting the page size of the scrollbar
    s" Scrollbar Page Size:" gtk_label_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

    0.0E+0  1.0E+0 1.0E+0 101.0E+0 1.0E+0 1.0E+0 gtk_adjustment_new to adj3
    
    \ Crashes the gtk-server:
    \ ['] cb_page_size s" cb_page_size"
    \ 2dup s" value_changed" adj3 widget>event gtk_server_connect 2drop
    \ events hct-insert
    
    adj3 gtk_hscale_new to hscale3
    
    0 hscale3 gtk_scale_set_digits
    
    0 true true hscale3 r@ gtk_box_pack_start
    
    hscale3 gtk_widget_show

    0 true true r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ box2


    gtk_hseparator_new >r
    
    0 true false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    10 false gtk_vbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    0 true false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show            \ box2

    s" Quit" gtk_button_new_with_label to exit-button
    
    0 true true exit-button r@ gtk_box_pack_start
    
    \ GTK_WIDGET_SET_FLAGS (button, GTK_CAN_DEFAULT);
    \ gtk_widget_grab_default (button);
    
    exit-button gtk_widget_show

    r>          gtk_widget_show   \ box1
    window      gtk_widget_show
  ;





  : example
    gtk_init                      \ Initialise toolkit
    
    create_range_controls
    
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        exit-button  OF true ENDOF
        chk-btn      OF set-draw-value                    false ENDOF
        top-menu     OF GTK_POS_TOP    set-value-position false ENDOF
        bottom-menu  OF GTK_POS_BOTTOM set-value-position false ENDOF
        left-menu    OF GTK_POS_LEFT   set-value-position false ENDOF
        right-menu   OF GTK_POS_RIGHT  set-value-position false ENDOF
        cont-menu    OF GTK_UPDATE_CONTINUOUS    set-update-policy false ENDOF
        discont-menu OF GTK_UPDATE_DISCONTINUOUS set-update-policy false ENDOF
        delayed-menu OF GTK_UPDATE_DELAYED       set-update-policy false ENDOF
        hscale2      OF set-scale-digits false ENDOF
        hscale3      OF set-page-size    false ENDOF
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
