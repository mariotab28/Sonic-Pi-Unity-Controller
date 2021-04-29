'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo Maria Cidoncha Perez and Mario Tabasco Vargas
Version 0.0
'''

require 'ostruct'

'''
============================
  Attributes Structures
============================
'''
''' Attribute structure of the SYNTH play action '''
SynthAttributes = Struct.new :action, :synth_name, :notes, :mode, :amp, :pan,
:attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level, :fx do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'synth'
    self.synth_name ||= 'beep'
    self.notes ||= [52]
    self.mode ||= 'tick'
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= 0
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
    # Effect
    self.fx ||= ''
  end
end

''' Attribute structure of the SAMPLE play action '''
SampleAttributes = Struct.new :action, :sample_name, :amp, :pan,
  :attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level,
  :lpf, :lpf_attack, :lpf_decay, :lpf_sustain, :lpf_release, :lpf_min, :lpf_init_level, :lpf_release_level, :lpf_sustain_level, :lpf_decay_level, :lpf_attack_level, :lpf_env_curve,
  :hpf, :hpf_max, :hpf_attack, :hpf_decay, :hpf_sustain, :hpf_release, :hpf_init_level, :hpf_release_level, :hpf_sustain_level, :hpf_decay_level, :hpf_attack_level, :hpf_env_curve,
  :rate, :start, :finish, :norm, :pitch, :window_size, :pitch_dis, :time_dis, :compress, :threshold, :clamp_time,
:slope_above, :slope_below, :relax_time, :fx do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sample'
    self.sample_name ||= 'bd_pure'
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= -1
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
    # Sample configuration
    # Low Pass Filter
    self.lpf ||= -1
    self.lpf_attack ||= self.attack
    self.lpf_decay ||= self.decay
    self.lpf_sustain ||= self.sustain
    self.lpf_release ||= self.release
    self.lpf_min||= 130
    self.lpf_init_level ||= self.lpf_min
    self.lpf_release_level ||= self.lpf
    self.lpf_sustain_level ||= self.lpf_release_level
    self.lpf_decay_level ||= self.lpf_sustain_level
    self.lpf_attack_level ||= self.lpf_decay_level
    self.lpf_env_curve ||= 2
    # High Pass Filter
    self.hpf ||= -1
    self.hpf_max ||= 200
    self.hpf_attack ||= self.attack
    self.hpf_decay ||= self.decay
    self.hpf_sustain ||= self.sustain
    self.hpf_release ||= self.release
    self.hpf_init_level ||= 130
    self.hpf_release_level ||= self.hpf
    self.hpf_sustain_level ||= self.hpf_release_level
    self.hpf_decay_level ||= self.hpf_sustain_level
    self.hpf_attack_level ||= self.hpf_decay_level
    self.hpf_env_curve ||= 2
    # Other sample attributes
    self.rate ||= 1
    self.start ||= 0
    self.finish ||= 1
    self.norm ||= 0
    self.pitch ||= 0
    self.window_size ||= 0.2
    self.pitch_dis ||= 0.0
    self.time_dis ||= 0.0
    self.compress ||= 0
    self.threshold ||= 0.2
    self.clamp_time ||= 0.01
    self.slope_above ||= 0.5
    self.slope_below ||= 1
    self.relax_time ||= 0.01
    # Effect    
    self.fx ||= ''
  end
end

''' Attribute structure of the SLEEP action '''
SleepAttributes = Struct.new :action, :sleep_duration do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sleep'
    self.sleep_duration ||= 1
  end
end


'''
============================
  Command Struct
============================
  Parameters:
  - loop_id: The index of the loop to which the command is attached
- com_id: The index of the command inside the loop command list
- com_attr: Structure containing the attributes of the command

'''

Command = Struct.new(:loop_id, :com_id, :com_attr) do
  # Initialize
  def initialize(*)
    super
    self.loop_id ||= -1
    self.com_id ||= -1
    self.loop_id ||= nil
  end
end

'''
============================
  Methods
============================
'''

'''
Waits for messages from Unity and adds the received commands
- id: Index of the listening loop
- commands: The list of commands of the listening loop
'''
def listenUnityCommand(id, commands)
  # Gets OSC message from Unity
  val = sync"/osc*/sonicpi/unity/trigger"

  if val[0] == "loop"
    return 0
  end

  if val[0] == "stop"
    comAttr = SleepAttributes.new("stop", -1)
    comId = 0
    com = Command.new(0, 0, comAttr)
    puts "Loop stopped"
    
    # Add the command to command list
    commands[comId] = com
  else
    # Run through each message received
    numComs = val[0]
    a = 0
    for k in 1..numComs do
      # Parse message to Command
      loopId = val[a + 1]
      if loopId != id 
          puts "WRONG LOOP!-> " + a.to_s
        return
      end
      comId = val[a + 2]
      i = a + 3
      puts "Received Command: " + val[i].to_s + "(" + i.to_s + ")"
      case val[i]
      # ACTION: SLEEP
      when "sleep"
        comAttr = SleepAttributes.new(val[i], val[i+1])
        a = i + 1
        # ACTION: PLAY SYNTH
      when "synth"
        numOfNotes = val[i+2] # Number of notes of the sequence to play
        notesToPlay = [] # List of notes to play
        # Add the notes from the message to notesToPlay
        bi = i + 3 # Index of the beginning of the note sequence
        ei = i + 3 + numOfNotes - 1 # Index of the end of the note sequence
        
        for ni in bi..ei do notesToPlay.push(val[ni]) end
        
        pre_i = i
        i = ei + 1
        comAttr = SynthAttributes.new()
        comAttr.action = val[pre_i]
        comAttr.synth_name = val[pre_i + 1]
        comAttr.notes = notesToPlay
        comAttr.mode = val[i]
        comAttr.amp = val[i + 1]
        comAttr.pan = val[i + 2]
        comAttr.attack = val[i + 3]
        comAttr.sustain = val[i + 4]
        comAttr.release = val[i + 5]
        comAttr.decay = val[i + 6]
        comAttr.attack_level = val[i + 7]
        comAttr.sustain_level = val[i + 8]
        comAttr.decay_level = val[i + 9]
        comAttr.fx = val[i + 10]
        a = a + 16 + numOfNotes
      # SAMPLE
      when "sample"
        comAttr = SampleAttributes.new()
        comAttr.action = val[i]
        comAttr.sample_name = val[i + 1]
        comAttr.amp = val[i + 2]
        comAttr.pan = val[i + 3]
        comAttr.attack = val[i + 4]
        comAttr.sustain = val[i + 5]
        comAttr.release = val[i + 6]
        comAttr.decay = val[i + 7]
        comAttr.attack_level = val[i + 8]
        comAttr.sustain_level = val[i + 9]
        comAttr.decay_level = val[i + 10]
        comAttr.lpf = val[i + 11]
        comAttr.lpf_attack = val[i + 12]
        comAttr.lpf_decay = val[i + 13]
        comAttr.lpf_sustain = val[i + 14]
        comAttr.lpf_release = val[i + 15]
        comAttr.lpf_min= val[i + 16]
        comAttr.lpf_init_level = val[i + 17]
        comAttr.lpf_release_level = val[i + 18]
        comAttr.lpf_sustain_level = val[i + 19]
        comAttr.lpf_decay_level = val[i + 20]
        comAttr.lpf_attack_level = val[i + 21]
        comAttr.lpf_env_curve = val[i + 22]
        comAttr.hpf = val[i + 23]
        comAttr.hpf_max = val[i + 24]
        comAttr.hpf_attack = val[i + 25]
        comAttr.hpf_decay = val[i + 26]
        comAttr.hpf_sustain = val[i + 27]
        comAttr.hpf_release = val[i + 28]
        comAttr.hpf_init_level = val[i + 29]
        comAttr.hpf_release_level = val[i + 30]
        comAttr.hpf_sustain_level = val[i + 31]
        comAttr.hpf_decay_level = val[i + 32]
        comAttr.hpf_attack_level = val[i + 33]
        comAttr.hpf_env_curve = val[i + 34]
        comAttr.rate = val[i + 35]
        comAttr.start = val[i + 36]
        comAttr.finish = val[i + 37]
        comAttr.norm = val[i + 38]
        comAttr.pitch = val[i + 39]
        comAttr.window_size = val[i + 40]
        comAttr.pitch_dis = val[i + 41]
        comAttr.time_dis = val[i + 42]
        comAttr.compress = val[i + 43]
        comAttr.threshold = val[i + 44]
        comAttr.clamp_time = val[i + 45]
        comAttr.slope_above = val[i + 46]
        comAttr.slope_below = val[i + 47]
        comAttr.relax_time = val[i + 48]
        comAttr.fx = val[i + 49]
        a = a + 49 + 3
      else
        comAttr = nil
        puts "ERROR: Unknown action name."
      end
        
        com = Command.new(loopId, comId, comAttr)
        # Add the command to command list
        commands[comId] = com
    end
  end
end
'''
Process the command list
- id: Index of the processing loop
- commands: The list of commands of the processing loop
'''
  def processCommands(id, commands)
    slept = false
    # Processes each command from the command list in order
    commands.each do |com|
      puts "Processing: " + com.com_attr.action.to_s
      case com.com_attr.action
      # ACTION: SLEEP
      when "sleep"
	sleep com.com_attr.sleep_duration
        slept = true
	# ACTION: PLAY SYNTH
      when "synth"
            if com.com_attr.notes.count > 0
              use_synth com.com_attr.synth_name
              if com.com_attr.mode == 'tick' then
                tickNote = com.com_attr.notes.tick
              elsif com.com_attr.mode == 'chord' then
                tickNote = com.com_attr.notes
              elsif com.com_attr.mode == 'choose' then
                tickNote = com.com_attr.notes.choose
              else
                puts "Error: Unknown synth play mode."
              end
              if com.com_attr.fx != ''
                with_fx com.com_attr.fx do
                  play tickNote, amp: com.com_attr.amp, pan: com.com_attr.pan,
                    attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release,
                    decay: com.com_attr.decay, attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level,
                    decay_level: com.com_attr.decay_level
                end
              else
                play tickNote, amp: com.com_attr.amp, pan: com.com_attr.pan,
                  attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release,
                  decay: com.com_attr.decay, attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level,
                  decay_level: com.com_attr.decay_level
              end
            end
	# ACTION: PLAY SAMPLE
      when "sample"
	if com.com_attr.fx != ''
	  with_fx com.com_attr.fx do
	    sample com.com_attr.sample_name, amp: com.com_attr.amp, pan: com.com_attr.pan, attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release, decay: com.com_attr.decay,
                attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level, decay_level: com.com_attr.decay_level
	  end
	else
	  sample com.com_attr.sample_name, amp: com.com_attr.amp, pan: com.com_attr.pan, attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release, decay: com.com_attr.decay,
              attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level, decay_level: com.com_attr.decay_level, pitch: com.com_attr.pitch
	end
	
	''' TODO: RESTO DE ATRIBUTOS '''
	# ACTION: WITH FX
      when "fx"
	'''TODO: reproducir secuencia de comandos dentro del bloque with_fx'''
	# STOP
      when "stop"
	stop
      else
	puts "ERROR: Unknown command name. Can't process command."
      end
    end
    # Check if there is one sleep at the end
    if !slept
      sleep 0.5 # Needs to sleep at least 0.01
    end
  end
  
  
  '''
============================
Listen/Play loops definition
============================
'''
  
  def doListenerLoop(id,commands)
    puts "Listener " + id.to_s
    # LISTENER LOOP
    in_thread do
      loop do
        #puts "id = " + id.to_s
        # Listen commands for command list 'id'
        listenUnityCommand(id, commands[id])
        #puts "Listening (" + id.to_s + ")..."
        sleep 0.1
      end
    end
  end
  
  def doPlayerLoop(id,commands)
    puts "Player " + id.to_s
      # Set the live_loop name
      loopName = "playerLoop#{id.to_s}"
      # PLAYER LOOP
      live_loop loopName do
        #puts "Playing (" + id.to_s + ")..."
        # Process command list number 'id'
        processCommands(id, commands[id])
    end
  end
  
  
  '''
========================================================
Variables and Initialization of the loop rack
========================================================
'''
  use_osc "localhost", 4560
  
  # Number of loops listening to Unity commands
  nLoops = 0
  # Command list
  commands = []
  
def addLoop(nLoops, commands)
  # Add new loop
  loopId = nLoops
  puts "Adding new loop! id: " + loopId.to_s

  commands[loopId] = []
  doListenerLoop(loopId, commands) # Starts listening loop
  doPlayerLoop(loopId, commands)   # Starts playing loop
end

def deleteLoop(id, commands)
  # Remove loop[id]
  puts "Removing loop with id: " + loopId.to_s

  commands.delete_at(id)
end

  # Listen for loop creation messages:
  while(true)
    val = sync"/osc*/sonicpi/unity/trigger"
    if val[0] == "loop"
      addLoop(nLoops, commands)
      nLoops = nLoops + 1
    elsif val[0] == "del_loop"
      deleteLoop(val[1], commands)
      nLoops = nLoops - 1
    end
  end

'''
  for i in 0..(nLoops - 1)
    puts i
    commands[i] = []
    doListenerLoop(i, commands) # Starts listening loop
    doPlayerLoop(i, commands)   # Starts playing loop
  end
'''
  
  
  
