\ ==============================================================================
\
\     gsv14_expl - the gtk-server spinbutton example in the ffl
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

    tos-create formatter
    
    
  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                  \ Widget / Events
  0 value exit-button
  0 value spinner1
  0 value spinner2
  0 value label
  0 value check1
  0 value check2
  0 value change2
  0 value int-button
  0 value float-button


  : change_digits  ( -- = Process the value change event from the spinner )
    spinner2 gtk_spin_button_get_value_as_int
    spinner1 gtk_spin_button_set_digits
  ;
  
  
  : toggle_snap    ( -- = Process the toggle snap event )
    check1   gtk_toggle_button_get_active
    spinner1 gtk_spin_button_set_snap_to_ticks
  
  ;

  : toggle_numeric ( -- = Process the toggle numeric event )
    check2   gtk_toggle_button_get_active
    spinner1 gtk_spin_button_set_numeric
  ;
  
  
  : set_int_value     ( -- = Process the get value event )
    formatter tos-rewrite
    
    spinner1 gtk_spin_button_get_value_as_int
    formatter tos-write-number
    formatter str-get label gtk_label_set_text
  ;
  
  
  : set_float_value     ( -- = Process the get value event )
    formatter tos-rewrite
    precision >r
    spinner1 gtk_spin_button_get_digits set-precision
    spinner1 gtk_spin_button_get_value 
    formatter tos-write-float
    formatter str-get label gtk_label_set_text
    r> set-precision
  ;
  

  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window

    s" Spin Button" window gtk_window_set_title

    5 false gtk_vbox_new >r         \ main_vbox
    10 window gtk_container_set_border_width
    r@ window gtk_container_add
    
    s" Not accelerated" gtk_frame_new >r \ frame 
      0 true true r@ r'@ gtk_box_pack_start
      
      0 false gtk_vbox_new >r     \ vbox
        5 r@ gtk_container_set_border_width
        r@ r'@ gtk_container_add
        
        \ Day, month, year spinners
        
        0 false gtk_hbox_new >r   \ hbox
          5 true true r@ r'@ gtk_box_pack_start
          
          0 false gtk_vbox_new >r \ vbox2
            5 true true r@ r'@ gtk_box_pack_start
            
            s" Day :" gtk_label_new >r
            0.5E+0 0.0E+0 r@ gtk_misc_set_alignment
            0 true false r> r@ gtk_box_pack_start
            
            0.0E+0 5.0E+0 1.0E+0 31.0E+0 1.0E+0 1.0E+0 gtk_adjustment_new >r
            0 0.0E+0 r> gtk_spin_button_new >r
            true r@ gtk_spin_button_set_wrap
            0 true false r> r> gtk_box_pack_start
          
          0 false gtk_vbox_new >r \ vbox2
            5 true true r@ r'@ gtk_box_pack_start
            
            s" Month :" gtk_label_new >r
            0.5E+0 0.0E+0 r@ gtk_misc_set_alignment
            0 true false r> r@ gtk_box_pack_start
            
            0.0E+0 5.0E+0 1.0E+0 12.0E+0 1.0E+0 1.0E+0 gtk_adjustment_new >r
            0 0.0E+0 r> gtk_spin_button_new >r
            true r@ gtk_spin_button_set_wrap
            0 true false r> r> gtk_box_pack_start
         
          0 false gtk_vbox_new >r \ vbox2
            5 true true r@ r'@ gtk_box_pack_start
            
            s" Year :" gtk_label_new >r
            0.5E+0 0.0E+0 r@ gtk_misc_set_alignment
            0 true false r> r@ gtk_box_pack_start
            
            0.0E+0 100.0E+0 1.0E+0 2100.0E+0 0.0E+0 1998.0E+0 gtk_adjustment_new >r
            0 0.0E+0 r> gtk_spin_button_new >r
            false r@ gtk_spin_button_set_wrap
            -1 55 r@ gtk_widget_set_size_request
            0 true false r> r> gtk_box_pack_start
          rdrop rdrop rdrop       \ hbox, vbox, frame

    s" Accelerated" gtk_frame_new >r  \ frame
      0 true true r@ r'@ gtk_box_pack_start
    
      0 false gtk_vbox_new >r     \ vbox 
        5 r@ gtk_container_set_border_width
        r@ r'@ gtk_container_add
        
        0 false gtk_hbox_new >r   \ hbox
          5 true false r@ r'@ gtk_box_pack_start
          
          0 false gtk_vbox_new >r \ vbox2
            5 true true r@ r'@ gtk_box_pack_start
            
            s" Value :" gtk_label_new >r
            0.5E+0 0.0E+0 r@ gtk_misc_set_alignment
            0 true false r> r@ gtk_box_pack_start
            
            0.0E+0 100.0E+0 0.5E+0 10000.0E+0 -10000.0E+0 0.0E+0 gtk_adjustment_new >r
            2 1.0E+0 r> gtk_spin_button_new to spinner1
            true spinner1 gtk_spin_button_set_wrap
            -1 100 spinner1 gtk_widget_set_size_request
            0 true false spinner1 r> gtk_box_pack_start
          
          0 false gtk_vbox_new >r  \ vbox2
            5 true true r@ r'@ gtk_box_pack_start
            
            s" Digits :" gtk_label_new >r
            0.5E+0 0.0E+0 r@ gtk_misc_set_alignment
            0 true false r> r@ gtk_box_pack_start


            0.0E+0 1.0E+0 1.0E+0 5.0E+0 1.0E+0 2.0E+0 gtk_adjustment_new >r
           
            16 g_malloc to change2  \ Reserve event
            
            0 0.0E+0 r> gtk_spin_button_new to spinner2
            change2  s" value-changed" spinner2 gsv+server-connect 2drop
            true spinner2 gtk_spin_button_set_wrap
            -1 100 spinner2 gtk_widget_set_size_request
            0 true false spinner2 r> gtk_box_pack_start
          rdrop
          
          
        s" Snap to 0.5-ticks" gtk_check_button_new_with_label to check1
        true check1 gtk_toggle_button_set_active
        0 true true check1 r@ gtk_box_pack_start
        
        s" Numeric only input mode" gtk_check_button_new_with_label to check2
        true check2 gtk_toggle_button_set_active
        0 true true check2 r@ gtk_box_pack_start
        
        s" " gtk_label_new to label
          
        0 false gtk_hbox_new >r
          5 true false r@ r'@ gtk_box_pack_start
          
          s" Value as Int" gtk_button_new_with_label to int-button
          5 true true int-button r@ gtk_box_pack_start
          
          s" Value as Float" gtk_button_new_with_label to float-button
          5 true true float-button r> gtk_box_pack_start
          
          0 true true label r@ gtk_box_pack_start
          s" 0" label gtk_label_set_text
      rdrop rdrop
      
    0 false gtk_hbox_new >r
      0 true false r@ r'@ gtk_box_pack_start
      
      s" Close" gtk_button_new_with_label to exit-button
      5 true true exit-button r> gtk_box_pack_start
    rdrop                         \ main_vbox
    window gtk_widget_show_all
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      
      event>widget CASE
        window       OF true ENDOF
        exit-button  OF true ENDOF
        change2      OF change_digits   false ENDOF
        check1       OF toggle_snap     false ENDOF
        check2       OF toggle_numeric  false ENDOF
        int-button   OF set_int_value   false ENDOF
        float-button OF set_float_value false ENDOF
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
